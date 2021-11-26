using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
{
    public static class EditSolutionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            var model = new EditSolutionModel(odsCode, state);

            model.Title.Should().Be($"{state.CatalogueItemName} information for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrderItem.Should().Be(state);
            model.OrderItem.ServiceRecipients.Should().BeEquivalentTo(model.OrderItem.ServiceRecipients.Where(m => m.Selected).ToList());
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoQuantityOnServiceRecipients_StateQuantity_IsApplied(
           string odsCode,
           CreateOrderItemModel state)
        {
            state.ServiceRecipients.ForEach(sr => sr.Quantity = null);

            var model = new EditSolutionModel(odsCode, state);

            model.OrderItem.ServiceRecipients.ForEach(oi => oi.Quantity.Should().Be(state.Quantity));
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoDeliveryDateOnServiceRecipients_StatePlannedDeliveryDate_IsApplied(
            string odsCode,
            CreateOrderItemModel state)
        {
            foreach (var sr in state.ServiceRecipients)
            {
                sr.DeliveryDate = null;
                sr.Year = string.Empty;
                sr.Month = string.Empty;
                sr.Year = string.Empty;
            }

            state.PlannedDeliveryDate = state.PlannedDeliveryDate.Value.Date;

            var model = new EditSolutionModel(odsCode, state);

            model.OrderItem.ServiceRecipients.ForEach(oi => oi.DeliveryDate.Should().Be(state.PlannedDeliveryDate));
        }
    }
}
