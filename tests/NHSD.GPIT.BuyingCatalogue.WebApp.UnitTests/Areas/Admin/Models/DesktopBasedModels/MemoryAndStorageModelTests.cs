using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DesktopBasedModels
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

            var desktopMemoryAndStorage = solution.ClientApplication.NativeDesktopMemoryAndStorage;

            actual.SelectedMemorySize.Should().Be(desktopMemoryAndStorage.MinimumMemoryRequirement);
            actual.StorageSpace.Should().Be(desktopMemoryAndStorage.StorageRequirementsDescription);
            actual.ProcessingPower.Should().Be(desktopMemoryAndStorage.MinimumCpu);
            actual.SelectedResolution.Should().Be(desktopMemoryAndStorage.RecommendedResolution);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryAndStorageModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
