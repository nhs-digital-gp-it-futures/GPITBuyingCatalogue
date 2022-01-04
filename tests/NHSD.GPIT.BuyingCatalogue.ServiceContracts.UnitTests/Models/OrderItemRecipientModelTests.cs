using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models
{
    public static class OrderItemRecipientModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void ValidateDeliveryDate_ValidDeliveryDate_ReturnsNoError(
            OrderItemRecipientModel model,
            DateTime commencementDate)
        {
            model.DeliveryDate = commencementDate.Date.AddMonths(42);

            var actual = model.ValidateDeliveryDate(commencementDate);

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void ValidateDeliveryDate_BeyondMonthsLimit_ReturnsError(
            OrderItemRecipientModel model,
            DateTime commencementDate)
        {
            model.DeliveryDate = commencementDate.Date.AddMonths(42).AddDays(1);

            var actual = model.ValidateDeliveryDate(commencementDate);

            actual.Should().Be("Planned delivery date must be within 42 months from the commencement date for this Call-off Agreement");
        }

        [Theory]
        [CommonAutoData]
        public static void ValidateDeliveryDate_DateInPast_ReturnsError(
            OrderItemRecipientModel model,
            DateTime commencementDate)
        {
            model.DeliveryDate = DateTime.UtcNow;

            var actual = model.ValidateDeliveryDate(commencementDate);

            actual.Should().Be("Planned delivery date must be in the future");
        }

        [Theory]
        [InlineData("1", "1", "2023", null)]
        [InlineData("", "1", "2023", "Planned delivery date must be a real date")]
        [InlineData("1", "", "2023", "Planned delivery date must be a real date")]
        [InlineData("1", "1", "", "Planned delivery date must be a real date")]
        [InlineData("29", "2", "2024", null)]
        [InlineData("29", "2", "2023", "Planned delivery date must be a real date")]
        public static void ValidateDeliveryDate_InvalidDates_ReturnError(string day, string month, string year, string expected)
        {
            var model = new OrderItemRecipientModel { Day = day, Month = month, Year = year };

            var actual = model.ValidateDeliveryDate(new DateTime(2021, 12, 25));

            actual.Should().Be(expected);
        }
    }
}
