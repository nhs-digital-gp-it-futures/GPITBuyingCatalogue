using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
{
    public static class MemoryAndStorageModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var actual = new MemoryAndStorageModel(catalogueItem);

            actual.MemorySizes.Should().BeEquivalentTo(Framework.Constants.SelectLists.MemorySizes);

            var mobileMemoryAndStorage = solution.ClientApplication.MobileMemoryAndStorage;

            actual.SelectedMemorySize.Should().Be(mobileMemoryAndStorage.MinimumMemoryRequirement);
            actual.Description.Should().Be(mobileMemoryAndStorage.Description);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryAndStorageModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
