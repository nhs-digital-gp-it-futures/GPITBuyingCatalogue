using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CommencementDate
{
    public static class CommencementDateModelTests
    {
        private const int InitialPeriod = 3;
        private const int MaximumTerm = 12;

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            DateTime commencementDate)
        {
            var model = new CommencementDateModel(odsCode, callOffId, commencementDate, InitialPeriod, MaximumTerm);

            model.Title.Should().Be("Timescales for Call-off Agreement");
            model.OdsCode.Should().Be(odsCode);
            model.Day.Should().Be(commencementDate.Day.ToString("00"));
            model.Month.Should().Be(commencementDate.Month.ToString("00"));
            model.Year.Should().Be(commencementDate.Year.ToString("00"));
            model.InitialPeriodValue.Should().Be(InitialPeriod);
            model.MaximumTermValue.Should().Be(MaximumTerm);
        }

        [Theory]
        [CommonAutoData]
        public static void CommencementDate_WithValidDate_ParsesSuccessfully(
            CommencementDateModel model)
        {
            var date = DateTime.UtcNow;

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var deliveryDate = model.CommencementDate!.Value;

            deliveryDate.Day.Should().Be(date.Day);
            deliveryDate.Month.Should().Be(date.Month);
            deliveryDate.Year.Should().Be(date.Year);
        }

        [Theory]
        [CommonAutoData]
        public static void CommencementDate_WithInvalidDate_DoesNotParse(
            CommencementDateModel model)
        {
            model.Day = model.Month = model.Year = null;

            model.CommencementDate.HasValue.Should().BeFalse();
        }
    }
}
