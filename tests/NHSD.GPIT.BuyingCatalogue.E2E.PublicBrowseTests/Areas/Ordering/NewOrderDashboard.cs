using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class NewOrderDashboard
        : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public NewOrderDashboard(LocalWebApplicationFactory factory)
            : base(factory, "order/organisation/03F/order/neworder")
        {
            BuyerLogin();
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
            CommonActions.PageTitle().Should().BeEquivalentTo("Order description");
        }
    }
}
