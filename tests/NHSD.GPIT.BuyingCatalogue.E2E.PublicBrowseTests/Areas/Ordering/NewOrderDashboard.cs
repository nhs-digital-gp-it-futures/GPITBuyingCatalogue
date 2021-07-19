using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class NewOrderDashboard
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" } };

        public NewOrderDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.NewOrder),
                  Parameters)
        {
        }

        [Fact]
        public void NewOrderDashboard_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("New Order");
            OrderingPages.OrderDashboard.TaskListDisplayed().Should().BeTrue();
            OrderingPages.OrderDashboard.OrderDescriptionLinkDisplayed().Should().BeTrue();
            OrderingPages.OrderDashboard.OrderDescriptionStatusDisplayed().Should().BeTrue();
        }

        [Fact]
        public void NewOrderDashboard_ClickOrderDescription()
        {
            OrderingPages.OrderDashboard.ClickOrderDescriptionLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.NewOrderDescription)).Should().BeTrue();
        }
    }
}
