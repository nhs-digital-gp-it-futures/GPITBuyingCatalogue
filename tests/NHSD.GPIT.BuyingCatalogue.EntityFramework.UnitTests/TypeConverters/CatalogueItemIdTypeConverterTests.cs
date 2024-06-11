using System;
using System.ComponentModel;
using System.Globalization;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.TypeConverters;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.TypeConverters
{
    public static class CatalogueItemIdTypeConverterTests
    {
        [Theory]
        [MockInlineAutoData(typeof(string), true)]
        [MockInlineAutoData(typeof(int), false)]
        public static void CanConvertFrom_ReturnsExpectedResult(
            Type sourceType,
            bool expectedResult,
            ITypeDescriptorContext context,
            CatalogueItemIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.CanConvertFrom(context, sourceType);

            actualResult.Should().Be(expectedResult);
        }

        [Theory]
        [MockAutoData]
        public static void ConvertFrom_InvalidId_ReturnsExpectedResult(
            ITypeDescriptorContext context,
            CatalogueItemIdTypeConverter typeConverter)
        {
            Assert.Throws<FormatException>(
                () => typeConverter.ConvertFrom(context, CultureInfo.InvariantCulture, "NotCatalogueItemId"));
        }

        [Theory]
        [MockAutoData]
        public static void ConvertFrom_ValidId_ReturnsExpectedResult(
            ITypeDescriptorContext context,
            CatalogueItemIdTypeConverter typeConverter)
        {
            var actualResult = typeConverter.ConvertFrom(context, CultureInfo.InvariantCulture, "1000-001");

            actualResult.Should().Be(new CatalogueItemId(1000, "001"));
        }
    }
}
