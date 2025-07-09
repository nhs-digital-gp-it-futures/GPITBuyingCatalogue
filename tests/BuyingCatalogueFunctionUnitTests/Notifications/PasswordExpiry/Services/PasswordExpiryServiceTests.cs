using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Notifications.PasswordExpiry.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BuyingCatalogueFunctionTests.Notifications.PasswordExpiry.Services;

public static class PasswordExpiryServiceTests
{
    [Fact]
    public static void Constructor_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new CompositeCustomization(new AutoNSubstituteCustomization(),
            new QueueServiceClientSubstituteCustomization()));
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(PasswordExpiryService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetUsersNearingPasswordExpiry_UserWithNoEvents_ReturnsExpectedUsers(
        AspNetUser user,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        user.Disabled = false;
        user.Events.Clear();
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetUsersNearingPasswordExpiry(DateTime.UtcNow);

        result.Should().Contain(x => x.Id == user.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetUsersNearingPasswordExpiry_DisabledUser_ReturnsExpectedUsers(
        AspNetUser user,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        user.Disabled = true;
        user.Events.Clear();
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetUsersNearingPasswordExpiry(DateTime.UtcNow);

        result.Should().NotContain(x => x.Id == user.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetUsersNearingPasswordExpiry_UserWithPartialEvents_ReturnsExpectedUsers(
        AspNetUser user,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        user.Disabled = false;
        user.Events = new List<AspNetUserEvent>
        {
            new((int)EventTypeEnum.PasswordEnteredThirdExpiryThreshold),
        };

        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetUsersNearingPasswordExpiry(DateTime.UtcNow);

        result.Should().Contain(x => x.Id == user.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetUsersNearingPasswordExpiry_UserWithAllEvents_ReturnsExpectedUsers(
        AspNetUser user,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        user.Disabled = false;
        user.Events = new List<AspNetUserEvent>
        {
            new((int)EventTypeEnum.PasswordEnteredThirdExpiryThreshold),
            new((int)EventTypeEnum.PasswordEnteredSecondExpiryThreshold),
            new((int)EventTypeEnum.PasswordEnteredFirstExpiryThreshold)
        };

        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetUsersNearingPasswordExpiry(DateTime.UtcNow);

        result.Should().NotContain(x => x.Id == user.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_CreatesNotification(
        AspNetUser user,
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        const PasswordExpiryEventTypeEnum eventType = PasswordExpiryEventTypeEnum.PasswordEnteredThirdExpiryThreshold;
        options.Value.Returns(queueOptions);

        user.Disabled = false;
        user.Events.Clear();
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.Raise(DateTime.UtcNow, user, eventType);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)eventType);
        notifications.Should().Contain(x => x.To == user.Email);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_Throws(
        AspNetUser user,
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] QueueServiceClient queueServiceClient,
        [Frozen] QueueClient queueClient,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        const PasswordExpiryEventTypeEnum eventType = PasswordExpiryEventTypeEnum.PasswordEnteredThirdExpiryThreshold;
        queueClient.SendMessageAsync(Arg.Any<string>()).ThrowsAsync(new Exception());
        queueServiceClient.GetQueueClient(Arg.Any<string>()).Returns(queueClient);
        options.Value.Returns(queueOptions);

        user.Disabled = false;
        user.Events.Clear();
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await FluentActions.Awaiting(() => service.Raise(DateTime.UtcNow, user, eventType)).Should().ThrowAsync<Exception>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_PublishesEvent(
        AspNetUser user,
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] QueueServiceClient queueServiceClient,
        [Frozen] QueueClient queueClient,
        [Frozen] BuyingCatalogueDbContext context,
        PasswordExpiryService service)
    {
        const PasswordExpiryEventTypeEnum eventType = PasswordExpiryEventTypeEnum.PasswordEnteredThirdExpiryThreshold;
        queueServiceClient.GetQueueClient(Arg.Any<string>()).Returns(queueClient);
        options.Value.Returns(queueOptions);

        user.Disabled = false;
        user.Events.Clear();
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.Raise(DateTime.UtcNow, user, eventType);

        await queueClient.Received().SendMessageAsync(Arg.Any<string>());
    }
}
