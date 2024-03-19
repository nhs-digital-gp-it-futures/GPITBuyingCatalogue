using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class CatalogueItemTypeTests
    {
        [Theory]
        [InlineData(CatalogueItemType.AdditionalService, "Additional Service")]
        [InlineData(CatalogueItemType.AssociatedService, "Associated Service")]
        [InlineData(CatalogueItemType.Solution, "Catalogue Solution")]
        public static void DisplayName_ReturnsExpectedName(CatalogueItemType itemType, string expectedName)
        {
            var displayName = itemType.DisplayName();

            displayName.Should().Be(expectedName);
        }
    }
}
