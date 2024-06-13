using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email;

public class EmailDomainServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetAllowedDomains_ReturnsDomains(
        List<EmailDomain> domains,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        context.EmailDomains.AddRange(domains);
        await context.SaveChangesAsync();

        var result = await service.GetAllowedDomains();

        result.Should().BeEquivalentTo(domains);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetAllowedDomain_ReturnsDomain(
        List<EmailDomain> domains,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        context.EmailDomains.AddRange(domains);
        await context.SaveChangesAsync();

        var emailDomain = domains.First();

        var result = await service.GetAllowedDomain(emailDomain.Id);

        result.Should().BeEquivalentTo(emailDomain);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddAllowedDomain_AddsDomain(
        string domain,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        await service.AddAllowedDomain(domain);

        context.EmailDomains.Any(d => d.Domain == domain).Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task DeleteAllowedDomain_DeletesDomain(
        EmailDomain domain,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        context.EmailDomains.Add(domain);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.DeleteAllowedDomain(domain.Id);

        context.EmailDomains.AsNoTracking().Any(d => d.Id == domain.Id).Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task DeleteAllowedDomain_InvalidDomain_Returns(
        EmailDomain domain,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        var originalCount = context.EmailDomains.AsNoTracking().Count();

        await service.DeleteAllowedDomain(domain.Id);

        context.EmailDomains.AsNoTracking().Count().Should().Be(originalCount);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Exists_ValidDomain(
        EmailDomain domain,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        context.EmailDomains.Add(domain);
        await context.SaveChangesAsync();

        var result = await service.Exists(domain.Domain);

        result.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Exists_InvalidDomain(
        EmailDomain domain,
        EmailDomainService service)
    {
        var result = await service.Exists(domain.Domain);

        result.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static Task IsAllowed_Null_Throws(
        EmailDomainService service) =>
        FluentActions.Awaiting(() => service.IsAllowed(null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task IsAllowed_NoAllowedDomains_False(
        string emailAddress,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        context.EmailDomains.RemoveRange(context.EmailDomains);
        context.SaveChanges();

        var result = await service.IsAllowed(emailAddress);

        result.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task IsAllowed_ExactMatch_True(
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        const string emailAddress = "test@nhs.net";
        const string emailDomain = "@nhs.net";

        context.EmailDomains.Add(new EmailDomain(emailDomain));
        context.SaveChanges();

        var result = await service.IsAllowed(emailAddress);

        result.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbInlineAutoData("test@subdomain.nhs.net")]
    [MockInMemoryDbInlineAutoData("test@another-subdomain.nhs.net")]
    public static async Task IsAllowed_WildcardSubDomain_True(
        string emailAddress,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        const string emailDomain = "@*.nhs.net";

        context.EmailDomains.Add(new EmailDomain(emailDomain));
        context.SaveChanges();

        var result = await service.IsAllowed(emailAddress);

        result.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbInlineAutoData("test@subdomain.nhs.net")]
    [MockInMemoryDbInlineAutoData("test@another-subdomain.nhs.net")]
    public static async Task IsAllowed_SubDomainNotAllowed_True(
        string emailAddress,
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        const string emailDomain = "@nhs.net";

        context.EmailDomains.Add(new EmailDomain(emailDomain));
        context.SaveChanges();

        var result = await service.IsAllowed(emailAddress);

        result.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task IsAllowed_NoMatch_False(
        [Frozen] BuyingCatalogueDbContext context,
        EmailDomainService service)
    {
        const string emailAddress = "test@gmail.com";
        const string emailDomain = "@nhs.net";

        context.EmailDomains.Add(new EmailDomain(emailDomain));
        context.SaveChanges();

        var result = await service.IsAllowed(emailAddress);

        result.Should().BeFalse();
    }
}
