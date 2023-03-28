using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EmailDomainManagement;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.EmailDomainManagement;

[Collection(nameof(AdminCollection))]
public class AddEmailDomain : AuthorityTestBase
{
    public AddEmailDomain(LocalWebApplicationFactory factory)
    : base(
        factory,
        typeof(EmailDomainManagementController),
        nameof(EmailDomainManagementController.AddEmailDomain))
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        CommonActions.PageTitle().Should().Be("Add an email domain".FormatForComparison());
        CommonActions.LedeText()
            .Should()
            .Be("Enter the email domain you want to add to the allow list.".FormatForComparison());

        CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        CommonActions.SaveButtonDisplayed().Should().BeTrue();

        CommonActions.ElementIsDisplayed(EmailDomainManagementObjects.EmailDomainInput).Should().BeTrue();
    }

    [Fact]
    public void ClickGoBackLink_NavigatesCorrectly()
    {
        CommonActions.ClickGoBackLink();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.Index))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickSave_AddsDomain()
    {
        const string emailDomain = "@*.nhs.net";

        CommonActions.ElementAddValue(EmailDomainManagementObjects.EmailDomainInput, emailDomain);

        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.Index))
            .Should()
            .BeTrue();

        using var context = GetEndToEndDbContext();
        var domain = context.EmailDomains.AsNoTracking().FirstOrDefault(d => d.Domain == emailDomain);

        domain.Should().NotBeNull();

        context.EmailDomains.Remove(domain!);
        context.SaveChanges();
    }

    [Fact]
    public void Duplicate_SetsError()
    {
        using var context = GetEndToEndDbContext();
        var existingDomain = context.EmailDomains.First();

        CommonActions.ElementAddValue(EmailDomainManagementObjects.EmailDomainInput, existingDomain.Domain);

        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.AddEmailDomain))
            .Should()
            .BeTrue();

        CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
    }

    [Fact]
    public void NoInput_SetsModelError()
    {
        CommonActions.ElementAddValue(EmailDomainManagementObjects.EmailDomainInput, string.Empty);

        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.AddEmailDomain))
            .Should()
            .BeTrue();

        CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
    }

    [Fact]
    public void InvalidFormat_SetsModelError()
    {
        CommonActions.ElementAddValue(EmailDomainManagementObjects.EmailDomainInput, "nhs.net");

        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.AddEmailDomain))
            .Should()
            .BeTrue();

        CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
    }
}
