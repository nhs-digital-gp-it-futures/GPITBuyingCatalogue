using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServiceRecipientsDate
{
    public static class SelectAdditionalServiceRecipientsDateModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate)
        {
            state.SkipPriceSelection = false;

            var model = new SelectAdditionalServiceRecipientsDateModel(state, defaultDeliveryDate);

            model.Title.Should().Be($"Planned delivery date of {state.CatalogueItemName} for {state.CallOffId}");
            model.CommencementDate.Should().Be(state.CommencementDate);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndPlannedDeliveryDate_PropertiesCorrectlySet(
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate,
            DateTime plannedDeliveryDate)
        {
            state.PlannedDeliveryDate = plannedDeliveryDate;

            var model = new SelectAdditionalServiceRecipientsDateModel(state, defaultDeliveryDate);

            model.Day.Should().Be(plannedDeliveryDate.Day.ToString("00"));
            model.Month.Should().Be(plannedDeliveryDate.Month.ToString("00"));
            model.Year.Should().Be(plannedDeliveryDate.Year.ToString("00"));
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndDefaultDeliveryDate_PropertiesCorrectlySet(
            CreateOrderItemModel state,
            DateTime plannedDeliveryDate)
        {
            state.PlannedDeliveryDate = plannedDeliveryDate;

            var model = new SelectAdditionalServiceRecipientsDateModel(state, null);

            model.Day.Should().Be(plannedDeliveryDate.Day.ToString("00"));
            model.Month.Should().Be(plannedDeliveryDate.Month.ToString("00"));
            model.Year.Should().Be(plannedDeliveryDate.Year.ToString("00"));
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndCommencementDate_PropertiesCorrectlySet(
            CreateOrderItemModel state,
            DateTime commencementDate)
        {
            state.PlannedDeliveryDate = null;
            state.CommencementDate = commencementDate;

            var model = new SelectAdditionalServiceRecipientsDateModel(state, null);

            model.Day.Should().Be(commencementDate.Day.ToString("00"));
            model.Month.Should().Be(commencementDate.Month.ToString("00"));
            model.Year.Should().Be(commencementDate.Year.ToString("00"));
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDate_WithValidDate_ParsesSuccessfully(
            SelectAdditionalServiceRecipientsDateModel model)
        {
            var date = DateTime.UtcNow;

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var deliveryDate = model.DeliveryDate!.Value;

            deliveryDate.Day.Should().Be(date.Day);
            deliveryDate.Month.Should().Be(date.Month);
            deliveryDate.Year.Should().Be(date.Year);
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDate_WithInvalidDate_DoesNotParse(
            SelectAdditionalServiceRecipientsDateModel model)
        {
            model.Day = model.Month = model.Year = null;

            model.DeliveryDate.HasValue.Should().BeFalse();
        }
    }
}
