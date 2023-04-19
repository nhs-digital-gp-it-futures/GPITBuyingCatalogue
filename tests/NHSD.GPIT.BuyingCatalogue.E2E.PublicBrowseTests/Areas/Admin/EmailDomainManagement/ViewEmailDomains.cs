using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EmailDomainManagement;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.EmailDomainManagement;

[Collection(nameof(AdminCollection))]
public class ViewEmailDomains : AuthorityTestBase
{
    public ViewEmailDomains(LocalWebApplicationFactory factory)
    : base(
        factory,
        typeof(EmailDomainManagementController),
        nameof(EmailDomainManagementController.Index))
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        CommonActions.PageTitle().Should().Be("Allowed email domains".FormatForComparison());
        CommonActions.LedeText()
            .Should()
            .Be("Add or delete email domains that a user can register with.".FormatForComparison());
        CommonActions.ElementIsDisplayed(EmailDomainManagementObjects.HomeBreadcrumb).Should().BeTrue();

        CommonActions.ElementIsDisplayed(EmailDomainManagementObjects.NoDomainsText).Should().BeFalse();
        CommonActions.ElementIsDisplayed(EmailDomainManagementObjects.DomainsTable).Should().BeTrue();

        using var context = GetEndToEndDbContext();
        var emailDomains = context.EmailDomains.AsNoTracking().ToList();
        context.EmailDomains.RemoveRange(context.EmailDomains.AsNoTracking());
        context.SaveChanges();

        Driver.Navigate().Refresh();

        CommonActions.ElementIsDisplayed(EmailDomainManagementObjects.NoDomainsText).Should().BeTrue();
        CommonActions.ElementIsDisplayed(EmailDomainManagementObjects.DomainsTable).Should().BeFalse();

        CommonActions.ContinueButtonDisplayed().Should().BeTrue();

        context.InsertRangeWithIdentity(emailDomains);
        context.SaveChanges();
    }

    [Fact]
    public void ClickGoBackLink_NavigatesCorrectly()
    {
        CommonActions.ClickLinkElement(EmailDomainManagementObjects.HomeBreadcrumb);

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickContinue_NavigatesCorrectly()
    {
        CommonActions.ClickContinue();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
            .Should()
            .BeTrue();
    }
}
