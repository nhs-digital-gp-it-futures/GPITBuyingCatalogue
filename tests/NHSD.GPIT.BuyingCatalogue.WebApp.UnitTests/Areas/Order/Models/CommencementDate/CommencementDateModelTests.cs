using System;
using System.Globalization;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CommencementDate
{
    public static class CommencementDateModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode, 
            CallOffId callOffId, 
            DateTime commencementDate
            )
        {
            var model = new CommencementDateModel(odsCode, callOffId, commencementDate);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Commencement date for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.Day.Should().Be(commencementDate.Day.ToString("00"));
            model.Month.Should().Be(commencementDate.Month.ToString("00"));
            model.Year.Should().Be(commencementDate.Year.ToString("00"));
        }

        [Theory]
        [InlineData("","","")]
        [InlineData("Invalid", "1", "2022")]
        [InlineData("1", "Invalid", "2022")]
        [InlineData("1", "1", "Invalid")]
        [InlineData("29", "2", "2022")]
        [InlineData("1", "13", "2022")]
        public static void ToDateTime_InvalidDate_ReturnsError(string day, string month, string year)
        {
            var model = new CommencementDateModel
            {
                Day = day,
                Month = month,
                Year = year,
            };

            var (_, error) = model.ToDateTime();

            error.Should().Be("Commencement date must be a real date");
        }

        [Theory]
        [InlineData("1", "1", "2022")]
        [InlineData("31", "7", "2022")]
        [InlineData("29", "2", "2024")]
        public static void ToDateTime_ValidDate_ReturnsDate(string day, string month, string year)
        {
            var model = new CommencementDateModel
            {
                Day = day,
                Month = month,
                Year = year,
            };

            (DateTime? dateTime, string error) = model.ToDateTime();

            Assert.Null(error);
            Assert.NotNull(dateTime);

            dateTime.Should().Be(DateTime.ParseExact($"{model.Day}/{model.Month}/{model.Year}", "d/M/yyyy", CultureInfo.InvariantCulture));
        }

        [Fact] public static void ToDateTime_DateMoreThan60DaysInPast_ReturnsError()
        {
            var oldDate = DateTime.UtcNow.AddDays(-60);

            var model = new CommencementDateModel
            {
                Day = oldDate.Day.ToString(),
                Month = oldDate.Month.ToString(),
                Year = oldDate.Year.ToString(),
            };

            var (_, error) = model.ToDateTime();

            error.Should().Be("Commencement date must be in the future or within the last 60 days");
        }

        [Fact]
        public static void ToDateTime_DayLessThan60DaysInPast_ReturnsDate()
        {
            var oldDate = DateTime.UtcNow.AddDays(-59);

            var model = new CommencementDateModel
            {
                Day = oldDate.Day.ToString(),
                Month = oldDate.Month.ToString(),
                Year = oldDate.Year.ToString(),
            };

            (DateTime? dateTime, string error) = model.ToDateTime();

            Assert.Null(error);
            Assert.NotNull(dateTime);
            dateTime.Should().Be(oldDate.Date);
        }
    }
}
