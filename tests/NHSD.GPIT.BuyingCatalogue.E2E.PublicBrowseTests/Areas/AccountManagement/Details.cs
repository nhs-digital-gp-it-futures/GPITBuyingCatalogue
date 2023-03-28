using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    [Collection(nameof(AccountManagementCollection))]
    public sealed class Details : AccountManagerTestBase
    {
        private const int OrganisationId = 176;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public Details(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ManageAccountController),
                  nameof(ManageAccountController.Details),
                  Parameters)
        {
        }

        [Fact]
        public async Task Details_OrganisationDetailsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbAddress = (await context.Organisations.FirstAsync(s => s.Id == OrganisationId)).Address;

            var pageAddress = AccountManagementPages.Details.GetAddress().ToList();

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

            var pageOdsCode = AccountManagementPages.Details.GetOdsCode();

            pageOdsCode.Should().Be(dbOdsCode);
        }

        [Fact]
        public void Details_AllLinksDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DetailsObjects.UserAccountsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DetailsObjects.RelatedOrganisationsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DetailsObjects.NominatedOrganisationsLink).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickHomeBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(WebApp.Controllers.HomeController),
                nameof(WebApp.Controllers.HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickUserAccountsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DetailsObjects.UserAccountsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickRelatedOrganisationsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DetailsObjects.RelatedOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.RelatedOrganisations)).Should().BeTrue();
        }

        [Fact]
        public void Details_ClickNominatedOrganisationsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DetailsObjects.NominatedOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.NominatedOrganisations)).Should().BeTrue();
        }
    }
}
