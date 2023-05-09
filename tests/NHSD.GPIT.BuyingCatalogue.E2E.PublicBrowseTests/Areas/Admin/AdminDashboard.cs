using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    [Collection(nameof(AdminCollection))]
    public sealed class AdminDashboard : AuthorityTestBase
    {
        public AdminDashboard(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(factory, typeof(HomeController), nameof(HomeController.Index), null, testOutputHelper)
        {
        }

        [Fact]
        public void AdminDashboard_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BannerObjects.BuyerOrganisationsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BannerObjects.SupplierOrganisationsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BannerObjects.CatalogueSolutionsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BannerObjects.LogOutLink).Should().BeTrue();

            CommonActions.ElementIsDisplayed(HomeObjects.ManageCatalogueSolutionsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(HomeObjects.ManageFrameworksLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(HomeObjects.ManageSupplierDefinedEpicsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(HomeObjects.ManageBuyerOrganisationsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(HomeObjects.ManageSupplierOrganisationsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(HomeObjects.ManageAllUsersLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(HomeObjects.ManageAllOrdersLink).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickBuyerOrganisationsBannerLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BannerObjects.BuyerOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickSupplierOrganisationsBannerLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BannerObjects.SupplierOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickCatalogueSolutionsBannerLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BannerObjects.CatalogueSolutionsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickLogOutBannerLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BannerObjects.LogOutLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(WebApp.Controllers.HomeController),
                nameof(WebApp.Controllers.HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickManageCatalogueSolutionsLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageCatalogueSolutionsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickManageSupplierDefinedEpicsLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageSupplierDefinedEpicsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickManageBuyerOrganisationsLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageBuyerOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickManageSupplierOrganisationsLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageSupplierOrganisationsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickManageAllUsersLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageAllUsersLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ClickManageAllOrdersLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageAllOrdersLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.Index)).Should().BeTrue();
        }
    }
}
