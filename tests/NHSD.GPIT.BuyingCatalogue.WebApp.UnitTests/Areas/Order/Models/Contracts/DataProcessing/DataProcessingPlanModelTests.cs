using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DataProcessing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class DataProcessingPlanModelTests
    {
        [Theory]
        [MockAutoData]
        public static void CreateDataProcessingPlan(
            ContractFlags contract)
        {
            contract.UseDefaultDataProcessing = true;
            DataProcessingPlanModel model = new DataProcessingPlanModel(contract);

            model.UseDefaultDataProcessing.Should().BeTrue();
        }
    }
}
