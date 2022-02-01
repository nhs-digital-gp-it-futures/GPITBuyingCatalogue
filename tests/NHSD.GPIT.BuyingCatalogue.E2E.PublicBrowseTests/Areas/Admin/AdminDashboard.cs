using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class AdminDashboard : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdminDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(HomeController),
                  nameof(HomeController.Index),
                  null)
        {
        }

        [Fact]
        public void ManageSuppliers_ManageSuppliersOrgLinkDisplayed()
        {
            AdminPages.AddSolution.ManageSuppliersLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ManageCatalogueSolutions_AddSolutionLinkDisplayed()
        {
            AdminPages.AddSolution.AddSolutionLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AdminDashboard_ManageSupplierDefinedEpicsLinkDisplayed()
        {
            CommonActions.ElementExists(By.LinkText("Manage Supplier defined Epics")).Should().BeTrue();
        }
    }
}
