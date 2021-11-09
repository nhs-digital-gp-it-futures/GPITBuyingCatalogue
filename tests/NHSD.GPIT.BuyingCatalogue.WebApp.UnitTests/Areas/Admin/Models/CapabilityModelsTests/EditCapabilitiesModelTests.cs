using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModelsTests
{
    public sealed class EditCapabilitiesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructing_SetsPropertiesAsExpected(
            AdditionalService additionalService,
            IReadOnlyList<CapabilityCategory> capabilityCategories)
        {
            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories);

            model.Name.Should().Be(additionalService.CatalogueItem.Name);
            model.SolutionName.Should().Be(additionalService.CatalogueItem.Name);
            model.CatalogueItemType.Should().Be(additionalService.CatalogueItem.CatalogueItemType.AsString(EnumFormat.DisplayName));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructing_WithExistingItemCapability_SetsCapabilityAsSelected(
            AdditionalService additionalService,
            CatalogueItemCapability catalogueItemCapability,
            CatalogueItemEpic catalogueItemEpic)
        {
            var epic = new Epic
            {
                Id = "Epic1",
                Name = "Epic 1",
                IsActive = true,
            };

            var capability = new Capability
            {
                Id = 1,
                Name = "Capability 1",
                Epics = new[] { epic },
            };

            var capabilityCategory = new CapabilityCategory
            {
                Id = 1,
                Name = "Category 1",
                Capabilities = new[] { capability },
            };

            catalogueItemCapability.CapabilityId = capability.Id;
            catalogueItemEpic.CapabilityId = capability.Id;
            catalogueItemEpic.EpicId = epic.Id;

            additionalService.CatalogueItem.CatalogueItemCapabilities.Add(catalogueItemCapability);
            additionalService.CatalogueItem.CatalogueItemEpics.Add(catalogueItemEpic);

            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, new List<CapabilityCategory> { capabilityCategory });

            model.CapabilityCategories.Should().ContainSingle();
            model.CapabilityCategories[0].Capabilities.Should().ContainSingle();
            model.CapabilityCategories[0].Capabilities[0].Id.Should().Be(capability.Id);
            model.CapabilityCategories[0].Capabilities[0].Selected.Should().BeTrue();
            model.CapabilityCategories[0].Capabilities[0].Epics.Should().ContainSingle();
            model.CapabilityCategories[0].Capabilities[0].Epics[0].Selected.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void Constructing_WithNoExistingItemCapabilities_SetsCapabilityCategoriesAsExpected(
            AdditionalService additionalService,
            IReadOnlyList<CapabilityCategory> capabilityCategories)
        {
            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories);

            model.CapabilityCategories.Should().NotBeEmpty();
            model.CapabilityCategories.Should().NotContain(capabilityCategory => capabilityCategory.Capabilities.Any(capability => capability.Selected));
        }
    }
}
