using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrganisationDashboard
        : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public OrganisationDashboard(LocalWebApplicationFactory factory)
            : base(factory, "order/organisation/03F")
        {
            BuyerLogin();
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
            CommonActions.PageTitle().Should().BeEquivalentTo("New Order");
        }
    }
}
