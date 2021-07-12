using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServiceRecipients
{
    public static class SelectAdditionalServiceRecipientsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments__NewOrder_PropertiesCorrectlySet(
            string odsCode,
            string selectionMode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;

            var model = new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services/select/additional-service");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(state.CallOffId);

            // TODO: ServiceRecipients
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments__ExistingOrder_PropertiesCorrectlySet(
            string odsCode,
            string selectionMode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = false;

            var model = new SelectAdditionalServiceRecipientsModel(odsCode, state, selectionMode);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services/{state.CatalogueItemId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(state.CallOffId);

            // TODO: ServiceRecipients
        }
    }
}
