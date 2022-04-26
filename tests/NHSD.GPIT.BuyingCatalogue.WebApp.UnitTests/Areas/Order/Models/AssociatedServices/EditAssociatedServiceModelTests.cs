using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class EditAssociatedServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CreateOrderItemModel state)
        {
            var model = new EditAssociatedServiceModel(internalOrgId, state);

            model.Title.Should().Be($"{state.CatalogueItemName} information for {state.CallOffId}");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.OrderItem.Should().Be(state);
            model.EstimationPeriod.Should().Be(state.EstimationPeriod);
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingExistingSolution_BackLinkCorrectlySet(
            string internalOrgId,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = false;

            var model = new EditAssociatedServiceModel(internalOrgId, state);

            model.BackLink.Should().Be($"/order/organisation/{internalOrgId}/order/{state.CallOffId}/associated-services");
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingNewSolution__AndSkipPrice_BackLinkCorrectlySet(
            string internalOrgId,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;
            state.SkipPriceSelection = true;

            var model = new EditAssociatedServiceModel(internalOrgId, state);

            model.BackLink.Should().Be($"/order/organisation/{internalOrgId}/order/{state.CallOffId}/associated-services/select/associated-service");
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingNewSolution__AndNotSkipPrice_BackLinkCorrectlySet(
            string internalOrgId,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;
            state.SkipPriceSelection = false;

            var model = new EditAssociatedServiceModel(internalOrgId, state);

            model.BackLink.Should().Be($"/order/organisation/{internalOrgId}/order/{state.CallOffId}/associated-services/select/associated-service/price");
        }
    }
}
