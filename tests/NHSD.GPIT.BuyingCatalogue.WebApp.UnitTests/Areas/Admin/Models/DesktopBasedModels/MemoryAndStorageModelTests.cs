using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DesktopBasedModels
{
    public static class MemoryAndStorageModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = new MemoryAndStorageModel(catalogueItem);

            actual.MemorySizes.Should().BeEquivalentTo(Framework.Constants.SelectLists.MemorySizes);

            var desktopMemoryAndStorage = catalogueItem.Solution.GetClientApplication().NativeDesktopMemoryAndStorage;

            actual.SelectedMemorySize.Should().Be(desktopMemoryAndStorage.MinimumMemoryRequirement);
            actual.StorageSpace.Should().Be(desktopMemoryAndStorage.StorageRequirementsDescription);
            actual.ProcessingPower.Should().Be(desktopMemoryAndStorage.MinimumCpu);
            actual.SelectedResolution.Should().Be(desktopMemoryAndStorage.RecommendedResolution);
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop");
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryAndStorageModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [CommonInlineAutoData("", false)]
        [CommonInlineAutoData(" ", false)]
        [CommonInlineAutoData(null, false)]
        [CommonInlineAutoData("2Gb", true)]
        public static void IsComplete_CorrectlySet_WhenMemorySizeSet(
            string selectedMemorySize,
            bool expectedCompletionState,
            MemoryAndStorageModel model)
        {
            model.SelectedMemorySize = selectedMemorySize;

            var actual = model.IsComplete;

            actual.Should().Be(expectedCompletionState);
        }

        [Theory]
        [CommonInlineAutoData("", false)]
        [CommonInlineAutoData(" ", false)]
        [CommonInlineAutoData(null, false)]
        [CommonInlineAutoData("Storage Space", true)]
        public static void IsComplete_CorrectlySet_WhenStorageSpaceSet(
            string storageSpace,
            bool expectedCompletionState,
            MemoryAndStorageModel model)
        {
            model.StorageSpace = storageSpace;

            var actual = model.IsComplete;

            actual.Should().Be(expectedCompletionState);
        }

        [Theory]
        [CommonInlineAutoData("", false)]
        [CommonInlineAutoData(" ", false)]
        [CommonInlineAutoData(null, false)]
        [CommonInlineAutoData("Processing Power", true)]
        public static void IsComplete_CorrectlySet_WhenProcessingPowerSet(
            string processingPower,
            bool expectedCompletionState,
            MemoryAndStorageModel model)
        {
            model.ProcessingPower = processingPower;

            var actual = model.IsComplete;

            actual.Should().Be(expectedCompletionState);
        }
    }
}
