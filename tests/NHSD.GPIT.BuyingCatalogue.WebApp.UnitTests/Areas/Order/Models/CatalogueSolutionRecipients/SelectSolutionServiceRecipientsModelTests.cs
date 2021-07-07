using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutionRecipients
{
    public static class SelectSolutionServiceRecipientsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_NewOrder_PropertiesCorrectlySet(
            string odsCode,
            string selectionMode,
            CreateOrderItemModel state
            )
        {
            state.IsNewSolution = true;

            var model = new SelectSolutionServiceRecipientsModel(odsCode, state, selectionMode);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/catalogue-solutions/select/solution");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(state.CallOffId);

            // TODO: ServiceRecipients
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_ExistingOrder_PropertiesCorrectlySet(
            string odsCode,
            string selectionMode,
            CreateOrderItemModel state
        )
        {
            state.IsNewSolution = false;

            var model = new SelectSolutionServiceRecipientsModel(odsCode, state, selectionMode);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/catalogue-solutions/{state.CatalogueItemId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(state.CallOffId);

            // TODO: ServiceRecipients
        }
    }
}
