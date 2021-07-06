using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class SelectAdditionalServiceRecipientsDateModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate)
        {
            state.SkipPriceSelection = false;

            var model = new SelectAdditionalServiceRecipientsDateModel(odsCode, state, defaultDeliveryDate);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services/select/additional-service/price");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Planned delivery date of {state.CatalogueItemName} for {state.CallOffId}");
            model.CommencementDate.Should().Be(state.CommencementDate);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndSkipPrice_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate)
        {
            state.SkipPriceSelection = true;

            var model = new SelectAdditionalServiceRecipientsDateModel(odsCode, state, defaultDeliveryDate);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services/select/additional-service/price/recipients");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Planned delivery date of {state.CatalogueItemName} for {state.CallOffId}");
            model.CommencementDate.Should().Be(state.CommencementDate);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndPlannedDeliveryDate_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate,
            DateTime plannedDeliveryDate)
        {
            state.PlannedDeliveryDate = plannedDeliveryDate;

            var model = new SelectAdditionalServiceRecipientsDateModel(odsCode, state, defaultDeliveryDate);

            model.Day.Should().Be(plannedDeliveryDate.Day.ToString("00"));
            model.Month.Should().Be(plannedDeliveryDate.Month.ToString("00"));
            model.Year.Should().Be(plannedDeliveryDate.Year.ToString("00"));
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndDefaultDeliveryDate_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state,
            DateTime plannedDeliveryDate)
        {
            state.PlannedDeliveryDate = plannedDeliveryDate;

            var model = new SelectAdditionalServiceRecipientsDateModel(odsCode, state, null);

            model.Day.Should().Be(plannedDeliveryDate.Day.ToString("00"));
            model.Month.Should().Be(plannedDeliveryDate.Month.ToString("00"));
            model.Year.Should().Be(plannedDeliveryDate.Year.ToString("00"));
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndCommencementDate_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state,
            DateTime commencementDate)
        {
            state.PlannedDeliveryDate = null;
            state.CommencementDate = commencementDate;

            var model = new SelectAdditionalServiceRecipientsDateModel(odsCode, state, null);

            model.Day.Should().Be(commencementDate.Day.ToString("00"));
            model.Month.Should().Be(commencementDate.Month.ToString("00"));
            model.Year.Should().Be(commencementDate.Year.ToString("00"));
        }
    }
}
