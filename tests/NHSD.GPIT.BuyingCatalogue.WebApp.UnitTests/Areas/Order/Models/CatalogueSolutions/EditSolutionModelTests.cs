using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

            model.BackLinkText.Should().Be("Go back");
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

        [Theory]
        [CommonAutoData]
        public static void WhenEditingExistingSolution_BackLinkCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = false;

            var model = new EditSolutionModel(odsCode, state);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/catalogue-solutions");
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Declarative, "/order/organisation/{0}/order/{1}/catalogue-solutions/select/solution/price/flat/declarative")]
        [CommonInlineAutoData(ProvisioningType.OnDemand, "/order/organisation/{0}/order/{1}/catalogue-solutions/select/solution/price/flat/ondemand")]
        [CommonInlineAutoData(ProvisioningType.Patient, "/order/organisation/{0}/order/{1}/catalogue-solutions/select/solution/price/recipients/date")]
        public static void WhenEditingNewSolution_BackLinkCorrectlySet(
            ProvisioningType provisioningType,
            string expectedBackLink,
            string odsCode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;
            state.CataloguePrice.ProvisioningType = provisioningType;

            var model = new EditSolutionModel(odsCode, state);

            model.BackLink.Should().Be(string.Format(expectedBackLink, odsCode, state.CallOffId));
        }
    }
}
