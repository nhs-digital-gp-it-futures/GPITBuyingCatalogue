using System;
using System.ComponentModel;
using System.Globalization;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.TypeConverters;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.TypeConverters
{
    public static class CallOffIdTypeConverterTests
    {
        [Theory]
        [CommonInlineAutoData(typeof(string), true)]
        [CommonInlineAutoData(typeof(int), false)]
        public static void CanConvertFrom_ReturnsExpectedResult(
            Type sourceType,
            bool expectedResult,
            ITypeDescriptorContext context,
            CallOffIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.CanConvertFrom(context, sourceType);

            actualResult.Should().Be(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static void ConvertFrom_InvalidId_ReturnsExpectedResult(
            ITypeDescriptorContext context,
            CallOffIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.ConvertFrom(context, CultureInfo.InvariantCulture, "NotCallOffId");

            actualResult.Should().Be(default(CallOffId));
        }

        [Theory]
        [CommonAutoData]
        public static void ConvertFrom_ValidId_ReturnsExpectedResult(
            ITypeDescriptorContext context,
            CallOffIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.ConvertFrom(context, CultureInfo.InvariantCulture, "C1000-01");

            actualResult.Should().Be(new CallOffId(1000, 1));
        }
    }
}
