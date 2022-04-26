﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServiceRecipients
{
    public static class SelectAdditionalServiceRecipientsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments__NewOrder_PropertiesCorrectlySet(
            string internalOrgId,
            string selectionMode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = true;
            state.SkipPriceSelection = false;

            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, selectionMode);

            model.BackLink.Should().Be($"/order/organisation/{internalOrgId}/order/{state.CallOffId}/additional-services/select/additional-service/price");
            model.Title.Should().Be($"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(state.CallOffId);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments__ExistingOrder_PropertiesCorrectlySet(
            string internalOrgId,
            string selectionMode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = false;

            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, selectionMode);

            model.BackLink.Should().Be($"/order/organisation/{internalOrgId}/order/{state.CallOffId}/additional-services/{state.CatalogueItemId}");
            model.Title.Should().Be($"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(state.CallOffId);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoSelectionMode_PropertiesCorrectlySet(
           string internalOrgId,
           CreateOrderItemModel state)
        {
            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, null);

            model.SelectionPrompt.Should().Be("Select all");
            model.SelectionParameter.Should().Be("all");
            model.ServiceRecipients.Should().BeEquivalentTo(state.ServiceRecipients);
        }

        [Theory]
        [CommonAutoData]
        public static void WithAllSelectionMode_PropertiesCorrectlySet(
            string internalOrgId,
            CreateOrderItemModel state)
        {
            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, "all");

            model.SelectionPrompt.Should().Be("Deselect all");
            model.SelectionParameter.Should().Be("none");
            model.ServiceRecipients.ForEach(r => r.Selected.Should().Be(true));
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoneSelectionMode_PropertiesCorrectlySet(
            string internalOrgId,
            CreateOrderItemModel state)
        {
            var model = new SelectAdditionalServiceRecipientsModel(internalOrgId, state, "none");

            model.SelectionPrompt.Should().Be("Select all");
            model.SelectionParameter.Should().Be("all");
            model.ServiceRecipients.ForEach(r => r.Selected.Should().Be(false));
        }
    }
}
