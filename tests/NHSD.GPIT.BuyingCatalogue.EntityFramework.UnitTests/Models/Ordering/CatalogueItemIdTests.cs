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
    }
}
