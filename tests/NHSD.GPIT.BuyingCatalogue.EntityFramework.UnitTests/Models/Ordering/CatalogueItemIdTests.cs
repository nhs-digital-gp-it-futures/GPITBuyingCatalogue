using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering
{
    public static class CatalogueItemIdTests
    {
        [Theory]
        [AutoData]
        public static void NextSolutionId_InvalidItemId_ThrowsException(int supplierId)
        {
            var catalogueItemId = new CatalogueItemId(supplierId, "invalid");

            Assert.Throws<FormatException>(() => catalogueItemId.NextSolutionId());
        }

        [Theory]
        [AutoData]
        public static void NextSolutionId_ValidItemId_ReturnsNextId(int supplierId)
        {
            var itemId = new Random().Next(1, 99);
            var catalogueItemId = new CatalogueItemId(supplierId, itemId.ToString("D3"));

            var actual = catalogueItemId.NextSolutionId();

            actual.Should().BeEquivalentTo(new CatalogueItemId(supplierId, (itemId + 1).ToString("D3")));
        }

        [Theory]
        [InlineAutoData("10000-001")]
        [InlineAutoData("10000-005A001")]
        [InlineAutoData("10000-S-ABC")]
        [InlineAutoData("10000-S005")]
        [InlineAutoData("10000-S")]
        public static void NextAssociatedServiceId_InvalidFormat_ThrowsFormatException(
            string catalogueItemId)
        {
            var parsedId = CatalogueItemId.ParseExact(catalogueItemId);

            FluentActions.Invoking(() => parsedId.NextAssociatedServiceId()).Should().Throw<FormatException>();
        }

        [Theory]
        [InlineAutoData("10000-S-001", "S-002")]
        [InlineAutoData("10000-S-005", "S-006")]
        public static void NextAssociatedServiceId_Valid_ReturnsExpected(
            string catalogueItemId,
            string expectedAssociatedServiceId)
        {
            var parsedId = CatalogueItemId.ParseExact(catalogueItemId);

            parsedId.NextAssociatedServiceId().Should().Be(new CatalogueItemId(parsedId.SupplierId, expectedAssociatedServiceId));
        }

        [Theory]
        [InlineAutoData("10000-001", true)]
        [InlineAutoData("10000-001A001", true)]
        [InlineAutoData("10000-S-001", true)]
        [InlineAutoData("", false)]
        [InlineAutoData("0-", false)]
        [InlineAutoData(null, false)]
        public static void TryParse_ReturnsExpectedParsingResult(
            string catalogueItemId,
            bool expectedResult) => CatalogueItemId.TryParse(catalogueItemId, out _).Should().Be(expectedResult);

        [Theory]
        [InlineAutoData("10000-001", "001")]
        [InlineAutoData("10000-001A001", "001A001")]
        [InlineAutoData("10000-S-001", "S-001")]
        public static void TryParse_ReturnsExpectedItemId(
            string catalogueItemId,
            string expectedResult)
        {
            _ = CatalogueItemId.TryParse(catalogueItemId, out var parsedId);

            parsedId.ItemId.Should().Be(expectedResult);
        }

        [Theory]
        [InlineAutoData("10000-001", 10000)]
        [InlineAutoData("10-001A001", 10)]
        [InlineAutoData("1-S-001", 1)]
        public static void TryParse_ReturnsExpectedSupplierId(
            string catalogueItemId,
            int expectedSupplierId)
        {
            _ = CatalogueItemId.TryParse(catalogueItemId, out var parsedId);

            parsedId.SupplierId.Should().Be(expectedSupplierId);
        }
    }
}
