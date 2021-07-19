using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrganisationDashboard
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" } };

        public OrganisationDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DashboardController),
                  nameof(DashboardController.Organisation),
                  Parameters)
        {
        }

        [Fact]
        public void OrganisationDashboard_OrganisationDashboard_SectionsCorrectlyDisplayed()
        {
            OrderingPages.OrganisationDashboard.CreateOrderButtonDisplayed().Should().BeTrue();
            OrderingPages.OrganisationDashboard.IncompleteOrderTableDisplayed().Should().BeTrue();
            OrderingPages.OrganisationDashboard.CompleteOrderTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_CreateOrder_Click()
        {
            OrderingPages.OrganisationDashboard.ClickCreateNewOrder();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.NewOrder)).Should().BeTrue();
        }
    }
}
