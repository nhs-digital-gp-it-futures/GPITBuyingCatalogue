using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class OrderScenarios : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
           : base(factory, typeof(DashboardController), nameof(DashboardController.Organisation), Parameters, testOutputHelper)
        {
        }

        [Fact]
        public void FirstTest_DoesThing()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectTriageOrderValue(EntityFramework.Ordering.Models.OrderTriageValue.Over250K);

            OrderingPages.OrderingTriage.SelectIdentifiedOrder();

            OrderingPages.StartOrder.ReadyToStart();
        }

        [Fact]
        public void OrderWithSolutionUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(EntityFramework.Ordering.Models.OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(false);

            OrderingPages.StepTwoAddSolutionsAndServices();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(EntityFramework.Ordering.Models.OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(false);

            OrderingPages.StepTwoAddSolutionsAndServices("Automated Arrivals", "Automated Arrivals – Engineering Half Day");
        }

        [Fact]
        public void OrderAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(false);

            OrderingPages.StepTwoAddSolutionsAndServices("Automated Arrivals", "Automated Arrivals – Engineering Half Day");
        }
    }
}
