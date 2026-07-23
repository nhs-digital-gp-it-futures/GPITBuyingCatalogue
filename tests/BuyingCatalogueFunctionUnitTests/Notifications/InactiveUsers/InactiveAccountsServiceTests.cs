using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Notifications.InactiveAccount;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Notifications.InactiveUsers;

public static class InactiveAccountsServiceTests
{
    [Fact]
    public static void Constructor_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new CompositeCustomization(new AutoNSubstituteCustomization(),
            new QueueServiceClientSubstituteCustomization()));
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(InactiveAccountsService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_NoInactiveUsers_ReturnsEmpty(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var user1Id = 1;
        var user1LastLoginDate = utcNow.AddMonths(-1);
        var user1LastLoginEvent = new AspNetUserLoginEvent { UserId = user1Id, Date = user1LastLoginDate };
        var user1 = new AspNetUser
        {
            Id = user1Id,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user1LastLoginEvent],
        };
        user1LastLoginEvent.User = user1;

        var user2Id = 2;
        var user2LastLoginDate = utcNow.AddMonths(-2);
        var user2LastLoginEvent = new AspNetUserLoginEvent { UserId = user2Id, Date = user2LastLoginDate };
        var user2 = new AspNetUser
        {
            Id = user2Id,
            FirstName = "user",
            LastName = "two",
            UserName = "user.two@email.com",
            NormalizedUserName = "USER.TWO@EMAIL.COM",
            Email = "user.two@email.com",
            NormalizedEmail = "USER.TWO@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user2LastLoginEvent],
        };
        user2LastLoginEvent.User = user2;

        var user3Id = 3;
        var user3LastLoginDate = utcNow.AddMonths(-3);
        var user3LastLoginEvent = new AspNetUserLoginEvent { UserId = user3Id, Date = user3LastLoginDate };
        var user3 = new AspNetUser
        {
            Id = user3Id,
            FirstName = "user",
            LastName = "three",
            UserName = "user.three@email.com",
            NormalizedUserName = "USER.THREE@EMAIL.COM",
            Email = "user.three@email.com",
            NormalizedEmail = "USER.THREE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user3LastLoginEvent],
        };
        user3LastLoginEvent.User = user3;

        var user4Id = 4;
        var user4LastLoginDate = utcNow.AddMonths(-4);
        var user4LastLoginEvent = new AspNetUserLoginEvent { UserId = user4Id, Date = user4LastLoginDate };
        var user4 = new AspNetUser
        {
            Id = user4Id,
            FirstName = "user",
            LastName = "four",
            UserName = "user.four@email.com",
            NormalizedUserName = "USER.FOUR@EMAIL.COM",
            Email = "user.four@email.com",
            NormalizedEmail = "USER.FOUR@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user4LastLoginEvent],
        };
        user4LastLoginEvent.User = user4;

        var user5Id = 5;
        var user5LastLoginDate = utcNow.AddMonths(-5);
        var user5LastLoginEvent = new AspNetUserLoginEvent { UserId = user5Id, Date = user5LastLoginDate };
        var user5 = new AspNetUser
        {
            Id = user5Id,
            FirstName = "user",
            LastName = "five",
            UserName = "user.five@email.com",
            NormalizedUserName = "USER.FIVE@EMAIL.COM",
            Email = "user.five@email.com",
            NormalizedEmail = "USER.FIVE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user5LastLoginEvent],
        };
        user5LastLoginEvent.User = user5;

        await context.AspNetUsers.AddRangeAsync(user1, user2, user3, user4, user5);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));
        result.Should().BeEmpty();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_WithInactiveUsers_ReturnsExpected(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var user1Id = 1;
        var user1LastLoginDate = utcNow.AddMonths(-1);
        var user1LastLoginEvent = new AspNetUserLoginEvent { UserId = user1Id, Date = user1LastLoginDate };
        var user1 = new AspNetUser
        {
            Id = user1Id,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user1LastLoginEvent],
        };
        user1LastLoginEvent.User = user1;

        var user2Id = 2;
        var user2LastLoginDate = utcNow.AddMonths(-2);
        var user2LastLoginEvent = new AspNetUserLoginEvent { UserId = user2Id, Date = user2LastLoginDate };
        var user2 = new AspNetUser
        {
            Id = user2Id,
            FirstName = "user",
            LastName = "two",
            UserName = "user.two@email.com",
            NormalizedUserName = "USER.TWO@EMAIL.COM",
            Email = "user.two@email.com",
            NormalizedEmail = "USER.TWO@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user2LastLoginEvent],
        };
        user2LastLoginEvent.User = user2;

        var user3Id = 3;
        var user3LastLoginDate = utcNow.AddMonths(-6);
        var user3LastLoginEvent = new AspNetUserLoginEvent { UserId = user3Id, Date = user3LastLoginDate };
        var user3 = new AspNetUser
        {
            Id = user3Id,
            FirstName = "user",
            LastName = "three",
            UserName = "user.three@email.com",
            NormalizedUserName = "USER.THREE@EMAIL.COM",
            Email = "user.three@email.com",
            NormalizedEmail = "USER.THREE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user3LastLoginEvent],
        };
        user3LastLoginEvent.User = user3;

        var user4Id = 4;
        var user4LastLoginDate = utcNow.AddMonths(-6).AddDays(31);
        var user4LastLoginEvent = new AspNetUserLoginEvent { UserId = user4Id, Date = user4LastLoginDate };
        var user4 = new AspNetUser
        {
            Id = user4Id,
            FirstName = "user",
            LastName = "four",
            UserName = "user.four@email.com",
            NormalizedUserName = "USER.FOUR@EMAIL.COM",
            Email = "user.four@email.com",
            NormalizedEmail = "USER.FOUR@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user4LastLoginEvent],
        };
        user4LastLoginEvent.User = user4;

        var user5Id = 5;
        var user5LastLoginDate = utcNow.AddMonths(-6).AddDays(29);
        var user5LastLoginEvent = new AspNetUserLoginEvent { UserId = user5Id, Date = user5LastLoginDate };
        var user5 = new AspNetUser
        {
            Id = user5Id,
            FirstName = "user",
            LastName = "five",
            UserName = "user.five@email.com",
            NormalizedUserName = "USER.FIVE@EMAIL.COM",
            Email = "user.five@email.com",
            NormalizedEmail = "USER.FIVE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user5LastLoginEvent],
        };
        user5LastLoginEvent.User = user5;

        await context.AspNetUsers.AddRangeAsync(user1, user2, user3, user4, user5);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));

        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        result.ElementAt(0).Email.Should().Be(user3.Email);
        result.ElementAt(0).LastLoginDate().Should().Be(user3LastLoginDate);
        result.ElementAt(1).Email.Should().Be(user5.Email);
        result.ElementAt(1).LastLoginDate().Should().Be(user5LastLoginDate);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_DeactivatedInactiveUsers_NotReturned(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var user1Id = 1;
        var user1LastLoginDate = utcNow.AddMonths(-8);
        var user1LastLoginEvent = new AspNetUserLoginEvent { UserId = user1Id, Date = user1LastLoginDate };
        var user1 = new AspNetUser
        {
            Id = user1Id,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = true,
            LoginEvents = [user1LastLoginEvent],
        };
        user1LastLoginEvent.User = user1;

        var user2Id = 2;
        var user2LastLoginDate = utcNow.AddMonths(-7);
        var user2LastLoginEvent = new AspNetUserLoginEvent { UserId = user2Id, Date = user2LastLoginDate };
        var user2 = new AspNetUser
        {
            Id = user2Id,
            FirstName = "user",
            LastName = "two",
            UserName = "user.two@email.com",
            NormalizedUserName = "USER.TWO@EMAIL.COM",
            Email = "user.two@email.com",
            NormalizedEmail = "USER.TWO@EMAIL.COM",
            Disabled = false,
            LoginEvents = [user2LastLoginEvent],
        };
        user2LastLoginEvent.User = user2;

        var user3Id = 3;
        var user3LastLoginDate = utcNow.AddMonths(-6);
        var user3LastLoginEvent = new AspNetUserLoginEvent { UserId = user3Id, Date = user3LastLoginDate };
        var user3 = new AspNetUser
        {
            Id = user3Id,
            FirstName = "user",
            LastName = "three",
            UserName = "user.three@email.com",
            NormalizedUserName = "USER.THREE@EMAIL.COM",
            Email = "user.three@email.com",
            NormalizedEmail = "USER.THREE@EMAIL.COM",
            Disabled = true,
            LoginEvents = [user3LastLoginEvent],
        };
        user3LastLoginEvent.User = user3;

        await context.AspNetUsers.AddRangeAsync(user1, user2, user3);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.ElementAt(0).Email.Should().Be(user2.Email);
        result.ElementAt(0).LastLoginDate().Should().Be(user2LastLoginDate);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_CreatedUserAccountNotLoggedIn_InsideThreshold_ReturnsExpected(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var user = new AspNetUser
        {
            Id = 1,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [],
            Created = utcNow.AddDays(-14),
        };

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));
        result.Should().BeEmpty();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_CreatedUserAccountNotLoggedIn_OutsideThreshold_ReturnsExpected(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var user = new AspNetUser
        {
            Id = 1,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [],
            Created = utcNow.AddMonths(-6),
        };

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.ElementAt(0).Email.Should().Be(user.Email);
        result.ElementAt(0).LastLoginDate().Should().Be(user.Created);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_NotDeactivated_HasSomeNotificationEvents_ReturnsExpected(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6).AddDays(1);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var notificationEvent1 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        var notificationEvent2 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold);
        var notificationEvent3 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold);
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [notificationEvent1, notificationEvent2, notificationEvent3],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));
        result.Should().HaveCount(1);
        result.ElementAt(0).Email.Should().Be(user.Email);
        result.ElementAt(0).LastLoginDate().Should().Be(user.LastLoginDate());
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInactiveAccounts_NotDeactivated_HasAllNotificationEvents_NotReturned(
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6).AddDays(1);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var notificationEvent1 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        var notificationEvent2 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold);
        var notificationEvent3 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold);
        var notificationEvent4 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredForthExpiryThreshold);
        var notificationEvent5 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredExpiredThreshold);
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [notificationEvent1, notificationEvent2, notificationEvent3, notificationEvent4, notificationEvent5],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetInactiveAccounts(DateOnly.FromDateTime(utcNow));
        result.Should().BeEmpty();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_4MonthsInactivity_DoesntCreateNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-4);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeFalse();
        updatedUser.DeactivationReason.Should().BeNull();
        updatedUser.Events.Should().BeEmpty();
        notifications.Should().NotContain(x => x.To == user.Email);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_30DaysFrom6MonthsInactivity_CreatesNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6).AddDays(30);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeFalse();
        updatedUser.DeactivationReason.Should().BeNull();
        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        notifications.Should().Contain(x => x.To == user.Email);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_14DaysFrom6MonthsInactivity_CreatesNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6).AddDays(14);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var notificationEvent1 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [notificationEvent1],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeFalse();
        updatedUser.DeactivationReason.Should().BeNull();
        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold);
        notifications.Should().Contain(x => x.To == user.Email);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_7DaysFrom6MonthsInactivity_CreatesNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6).AddDays(7);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var notificationEvent1 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        var notificationEvent2 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold);
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [notificationEvent1, notificationEvent2],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeFalse();
        updatedUser.DeactivationReason.Should().BeNull();
        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold);
        notifications.Should().Contain(x => x.To == user.Email);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_1DayFrom6MonthsInactivity_CreatesNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6).AddDays(1);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var notificationEvent1 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        var notificationEvent2 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold);
        var notificationEvent3 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold);
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [notificationEvent1, notificationEvent2, notificationEvent3],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeFalse();
        updatedUser.DeactivationReason.Should().BeNull();
        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)InactiveAccountEventType.InactivityEnteredForthExpiryThreshold);
        notifications.Should().Contain(x => x.To == user.Email);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_6MonthsInactivity_CreatesDeactivationNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddMonths(-6);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var notificationEvent1 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold);
        var notificationEvent2 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold);
        var notificationEvent3 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold);
        var notificationEvent4 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredForthExpiryThreshold);
        var notificationEvent5 = new AspNetUserEvent((int)InactiveAccountEventType.InactivityEnteredExpiredThreshold);
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [notificationEvent1, notificationEvent2, notificationEvent3, notificationEvent4, notificationEvent5],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeTrue();
        updatedUser.DeactivationReason.Should().Be(AccountDeactivationReason.Inactivity);
        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)InactiveAccountEventType.InactivityEnteredExpiredThreshold);
        notifications.Should().Contain(x => x.To == user.Email && x.EmailNotificationType == EmailNotificationTypeEnum.AccountDeactivation);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Raise_Passed6MonthsInactivity_WithNoNotificationsSoFar_CreatesNotification(
        QueueOptions queueOptions,
        [Frozen] IOptions<QueueOptions> options,
        [Frozen] BuyingCatalogueDbContext context,
        InactiveAccountsService service)
    {
        options.Value.Returns(queueOptions);
        var utcNow = DateTime.UtcNow;

        var userId = 1;
        var userLastLoginDate = utcNow.AddYears(-1).AddMonths(-6);
        var userLastLoginEvent = new AspNetUserLoginEvent { UserId = userId, Date = userLastLoginDate };
        var user = new AspNetUser
        {
            Id = userId,
            FirstName = "user",
            LastName = "one",
            UserName = "user.one@email.com",
            NormalizedUserName = "USER.ONE@EMAIL.COM",
            Email = "user.one@email.com",
            NormalizedEmail = "USER.ONE@EMAIL.COM",
            Disabled = false,
            LoginEvents = [userLastLoginEvent],
            Events = [],
        };
        userLastLoginEvent.User = user;

        await context.AspNetUsers.AddAsync(user);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var defaultEmailPreference = GetEmailPreferenceType(user);

        await service.Raise(user, DateOnly.FromDateTime(utcNow), defaultEmailPreference);

        var updatedUser = await context.AspNetUsers.Include(x => x.Events).FirstAsync(x => x.Id == user.Id);
        var notifications = await context.EmailNotifications.ToListAsync();

        updatedUser.Disabled.Should().BeTrue();
        updatedUser.DeactivationReason.Should().Be(AccountDeactivationReason.Inactivity);
        updatedUser.Events.Should().Contain(x => x.EventTypeId == (int)InactiveAccountEventType.InactivityEnteredExpiredThreshold);
        notifications.Should().Contain(x => x.To == user.Email);
    }

    private static EmailPreferenceType GetEmailPreferenceType(AspNetUser user)
    {
        return new EmailPreferenceType()
        {
            Id = (int)EmailPreferenceTypeEnum.InactiveAccount,
            Name = "InactiveAccount",
            RoleType = EmailPreferenceRoleType.All,
            UserPreferences = 
            [
                new UserEmailPreference()
                {
                    UserId = user.Id,
                    EmailPreferenceTypeId = (int)EmailPreferenceTypeEnum.InactiveAccount,
                    Enabled = true,
                }
            ],
            DefaultEnabled = true,
            SupportedEventTypes =
            [
                new EventType()
                {
                    Id = 6,
                    Name = "InactivityEnteredFirstExpiryThreshold",
                    EmailPreferenceTypeId = (int)EmailPreferenceTypeEnum.InactiveAccount,
                },
                new EventType()
                {
                    Id = 7,
                    Name = "InactivityEnteredSecondExpiryThreshold",
                    EmailPreferenceTypeId = (int)EmailPreferenceTypeEnum.InactiveAccount,
                },
                new EventType()
                {
                    Id = 8,
                    Name = "InactivityEnteredThirdExpiryThreshold",
                    EmailPreferenceTypeId = (int)EmailPreferenceTypeEnum.InactiveAccount,
                },
                new EventType()
                {
                    Id = 9,
                    Name = "InactivityEnteredFourthExpiryThreshold",
                    EmailPreferenceTypeId = (int)EmailPreferenceTypeEnum.InactiveAccount,
                },
            ],
        };
    }
}
