using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class EditAdditionalServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            var model = new EditAssociatedServiceModel(odsCode, state);

            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"{state.CatalogueItemName} associated service information for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrderItem.Should().Be(state);
            model.EstimationPeriod.Should().Be(state.EstimationPeriod);
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingExistingSolution_BackLinkCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = false;

            var model = new EditAssociatedServiceModel(odsCode, state);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services");
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingNewSolution__AndSkipPrice_BackLinkCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;
            state.SkipPriceSelection = true;

            var model = new EditAssociatedServiceModel(odsCode, state);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services/select/associated-service");
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingNewSolution__AndNotSkipPrice_BackLinkCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;
            state.SkipPriceSelection = false;

            var model = new EditAssociatedServiceModel(odsCode, state);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/associated-services/select/associated-service/price");
        }
    }
}
