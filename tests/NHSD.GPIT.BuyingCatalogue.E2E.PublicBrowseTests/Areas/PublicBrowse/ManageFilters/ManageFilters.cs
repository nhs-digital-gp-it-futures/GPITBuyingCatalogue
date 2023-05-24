using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.ManageFilters
{
    [Collection(nameof(SharedContextCollection))]
    public class ManageFilters : BuyerTestBase
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public ManageFilters(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageFiltersController), nameof(ManageFiltersController.Index), null, null, Parameters)
        {
        }

        [Fact]
        public async Task ManageFilters_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.BuyerDashboardBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.CreateNewFilterLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FiltersTable).Should().BeTrue();
            CommonActions.ElementExists(ManageFilterObjects.NoFiltersMessage).Should().BeFalse();

            var filter = await GetUserFilter();
            CommonActions.ElementTextEqualTo(ManageFilterObjects.FilterName, filter.Name);
            CommonActions.ElementTextEqualTo(ManageFilterObjects.FilterDescription, filter.Description);
            CommonActions.ElementTextEqualTo(ManageFilterObjects.FilterLastUpdated, filter.LastUpdated.ToShortDateString());
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterViewLink).Should().BeTrue();
        }

        [Fact]
        public void ManageFilters_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageFilters_ClickOrganisationDetailsBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.BuyerDashboardBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BuyerDashboardController),
                nameof(BuyerDashboardController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ManageFilters_ClickAddFilterLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(ManageFilterObjects.CreateNewFilterLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterCapabilities)).Should().BeTrue();
        }

        [Fact]
        public void ManageFilters_ClickViewFilterLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(ManageFilterObjects.FilterViewLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.FilterDetails)).Should().BeTrue();
        }

        [Fact]
        public void ManageFilters_ClickGoBackLink_ExpectedResult()
        {
            var filterId = 1;
            var parameters = new Dictionary<string, string>()
            {
                { nameof(filterId), filterId.ToString() },
            };

            NavigateToUrl(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.FilterDetails),
                parameters,
                parameters);
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ManageFilters_FilterDetails_DisplaysCorrectPage()
        {
            var filterId = 1;
            var parameters = new Dictionary<string, string>()
            {
                { nameof(filterId), filterId.ToString() },
            };

            NavigateToUrl(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.FilterDetails),
                parameters,
                parameters);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsNameAndDescription).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsCapabilities).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsCapabilitiesAndEpics).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsAdditionalFilters).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsViewSolutions).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsViewLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageFilterObjects.FilterDetailsDeleteLink).Should().BeTrue();
        }

        private async Task<AspNetUser> GetUser()
        {
            await using var context = GetEndToEndDbContext();

            return context.Users.Include(x => x.PrimaryOrganisation)
                .First(x => x.Id == UserSeedData.SueId);
        }

        private async Task<Filter> GetUserFilter()
        {
            await using var context = GetEndToEndDbContext();

            var currentUser = await GetUser();
            return context.Filters.Include(x => x.Organisation)
                .First(x => x.OrganisationId == currentUser.PrimaryOrganisationId);
        }
    }
}
