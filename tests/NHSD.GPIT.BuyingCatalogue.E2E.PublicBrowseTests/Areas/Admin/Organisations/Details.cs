using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    [Collection(nameof(AdminCollection))]
    public sealed class Details : AuthorityTestBase
    {
        private const int OrganisationId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public Details(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrganisationsController),
                  nameof(OrganisationsController.Details),
                  Parameters)
        {
        }

        [Fact]
        public async Task Details_OrganisationDetailsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbAddress = (await context.Organisations.FirstAsync(s => s.Id == OrganisationId)).Address;

            var pageAddress = AdminPages.Details.GetAddress().ToList();

            foreach (var prop in dbAddress.GetType().GetProperties())
            {
                if (prop.GetValue(dbAddress) is null)
                    continue;

                var value = prop.GetValue(dbAddress)?.ToString();
                pageAddress.First().Should().Contain(value);
            }
        }

        [Fact]
        public async Task Details_OdsCodeDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbOdsCode = (await context.Organisations.FirstAsync(s => s.Id == OrganisationId)).ExternalIdentifier;

            var pageOdsCode = AdminPages.Details.GetOdsCode();

            pageOdsCode.Should().Be(dbOdsCode);
        }

        [Fact]
        public void Details_AllLinksDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.ManageBuyerOrganisationsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.UserAccountsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.RelatedOrganisationsLink).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickHomeBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickManageBuyerOrganisationsBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonObjects.ManageBuyerOrganisationsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickUserAccountsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.UserAccountsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Users)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickRelatedOrganisationsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.RelatedOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.RelatedOrganisations)).Should().BeTrue();
        }
    }
}
