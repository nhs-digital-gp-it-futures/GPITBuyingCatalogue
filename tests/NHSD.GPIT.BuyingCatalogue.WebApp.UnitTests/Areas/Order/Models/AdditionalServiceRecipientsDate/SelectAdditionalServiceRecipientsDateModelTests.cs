using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServiceRecipientsDate
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

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services/select/additional-service/price/recipients");
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

        [Fact]
        public static void ValidateAndGetDeliveryDate_ValidDeliveryDate_ReturnsNoError()
        {
            var model = new SelectAdditionalServiceRecipientsDateModel { Day = "1", Month = "2", Year = "2022", CommencementDate = new DateTime(2021, 12, 25) };

            (var date, var error) = model.ValidateAndGetDeliveryDate();

            error.Should().BeNull();
            date.Should().Be(new DateTime(2022, 2, 1));
        }

        [Fact]
        public static void ValidateAndGetDeliveryDate_BeyondMonthsLimit_ReturnsError()
        {
            var commencementDate = new DateTime(2021, 12, 25);
            var badDeliveryDate = commencementDate.AddMonths(42).AddDays(1);
            var model = new SelectAdditionalServiceRecipientsDateModel
            {
                Day = badDeliveryDate.Day.ToString(),
                Month = badDeliveryDate.Month.ToString(),
                Year = badDeliveryDate.Year.ToString(),
                CommencementDate = commencementDate,
            };

            (var date, var error) = model.ValidateAndGetDeliveryDate();

            error.Should().Be("Planned delivery date must be within 42 months from the commencement date for this Call-off Agreement");
            date.Should().BeNull();
        }

        [Theory]
        [InlineData("1", "1", "2022", null)]
        [InlineData("", "1", "2022", "Planned delivery date must be a real date")]
        [InlineData("1", "", "2022", "Planned delivery date must be a real date")]
        [InlineData("1", "1", "", "Planned delivery date must be a real date")]
        [InlineData("29", "2", "2024", null)]
        [InlineData("29", "2", "2023", "Planned delivery date must be a real date")]
        public static void ValidateAndGetDeliveryDate_InvalidDates_ReturnError(string day, string month, string year, string expected)
        {
            var model = new SelectAdditionalServiceRecipientsDateModel
            {
                Day = day,
                Month = month,
                Year = year,
                CommencementDate = new DateTime(2021, 12, 25),
            };

            (_, var error) = model.ValidateAndGetDeliveryDate();

            error.Should().Be(expected);
        }
    }
}
