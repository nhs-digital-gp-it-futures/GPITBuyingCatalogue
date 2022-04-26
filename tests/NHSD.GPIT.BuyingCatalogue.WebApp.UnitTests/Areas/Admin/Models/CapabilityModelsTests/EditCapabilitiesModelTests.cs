using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModelsTests
{
    public sealed class EditCapabilitiesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructing_SetsPropertiesAsExpected(
            Solution solution,
            AdditionalService additionalService,
            IReadOnlyList<CapabilityCategory> capabilityCategories)
        {
            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories)
            {
                SolutionName = solution.CatalogueItem.Name,
            };

            model.Title.Should().Be($"{additionalService.CatalogueItem.Name} Capabilities and Epics");
            model.SolutionName.Should().Be(solution.CatalogueItem.Name);
            model.CatalogueItemType.Should().Be(additionalService.CatalogueItem.CatalogueItemType.AsString(EnumFormat.DisplayName));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructing_WithSolution_SetsPropertiesAsExpected(
            Solution solution,
            IReadOnlyList<CapabilityCategory> capabilityCategories)
        {
            var model = new EditCapabilitiesModel(solution.CatalogueItem, capabilityCategories)
            {
                SolutionName = solution.CatalogueItem.Name,
            };

            model.Title.Should().Be($"Capabilities and Epics");
            model.SolutionName.Should().Be(solution.CatalogueItem.Name);
            model.CatalogueItemType.Should().Be(solution.CatalogueItem.CatalogueItemType.AsString(EnumFormat.DisplayName));
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
            List<CapabilityCategory> capabilityCategories,
            List<Capability> capabilities)
        {
            capabilityCategories.ForEach(cc => cc.Capabilities.AddRange(capabilities));

            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories);

            model.CapabilityCategories.Should().NotBeEmpty();
            model.CapabilityCategories.Should().NotContain(capabilityCategory => capabilityCategory.Capabilities.Any(capability => capability.Selected));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructing_CapabilityCategoryWithNoCapabilities_DoesNotAdd(
            AdditionalService additionalService)
        {
            var capabilityCategories = new List<CapabilityCategory>
            {
                new()
                {
                    Name = "First Category",
                    Capabilities = new List<Capability>(),
                },
                new()
                {
                    Name = "Second Category",
                    Capabilities = new List<Capability>()
                    {
                        new()
                        {
                            Name = "First Capability",
                        },
                    },
                },
            };

            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories);

            model.CapabilityCategories.Should().NotBeEmpty();
            model.CapabilityCategories.Should().Contain(capabilityCategory => string.Equals(capabilityCategory.Name, capabilityCategories[1].Name));
            model.CapabilityCategories.Should().NotContain(capabilityCategory => string.Equals(capabilityCategory.Name, capabilityCategories[0].Name));
        }
    }
}
