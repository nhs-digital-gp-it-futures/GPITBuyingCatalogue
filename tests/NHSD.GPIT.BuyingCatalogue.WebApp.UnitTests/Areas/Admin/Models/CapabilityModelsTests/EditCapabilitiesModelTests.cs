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
        [MockAutoData]
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
        [MockAutoData]
        public static void Constructing_WithSolution_SetsPropertiesAsExpected(
            Solution solution,
            IReadOnlyList<CapabilityCategory> capabilityCategories)
        {
            var model = new EditCapabilitiesModel(solution.CatalogueItem, capabilityCategories)
            {
                SolutionName = solution.CatalogueItem.Name,
            };

            model.Title.Should().Be("Capabilities and Epics");
            model.SolutionName.Should().Be(solution.CatalogueItem.Name);
            model.CatalogueItemType.Should().Be(solution.CatalogueItem.CatalogueItemType.AsString(EnumFormat.DisplayName));
        }

        [Theory]
        [MockAutoData]
        public static void Constructing_WithExistingItemCapability_SetsCapabilityAsSelected(
            AdditionalService additionalService,
            CatalogueItemCapability catalogueItemCapability)
        {
            var mustEpic = new Epic
            {
                Id = "Epic1",
                Name = "Epic 1",
                IsActive = true,
            };

            var mayEpic = new Epic
            {
                Id = "Epic2",
                Name = "Epic 2",
                IsActive = true,
            };

            var capability = new Capability
            {
                Id = 1,
                Name = "Capability 1",
                CapabilityEpics = new[]
                {
                    new CapabilityEpic()
                    {
                        Epic = mustEpic,
                        CompliancyLevel = CompliancyLevel.Must,
                    },
                    new CapabilityEpic()
                    {
                        Epic = mayEpic,
                        CompliancyLevel = CompliancyLevel.May,
                    },
                },
            };

            var capabilityCategory = new CapabilityCategory
            {
                Id = 1,
                Name = "Category 1",
                Capabilities = new[] { capability },
            };

            catalogueItemCapability.CapabilityId = capability.Id;

            additionalService.CatalogueItem.CatalogueItemCapabilities.Add(catalogueItemCapability);
            additionalService.CatalogueItem.CatalogueItemEpics.Add(
                new(
                    additionalService.CatalogueItemId,
                    capability.Id,
                    mustEpic.Id));
            additionalService.CatalogueItem.CatalogueItemEpics.Add(
                new(
                    additionalService.CatalogueItemId,
                    capability.Id,
                    mayEpic.Id));

            var model = new EditCapabilitiesModel(additionalService.CatalogueItem, new List<CapabilityCategory> { capabilityCategory });

            model.CapabilityCategories.Should().ContainSingle();
            model.CapabilityCategories[0].Capabilities.Should().ContainSingle();
            model.CapabilityCategories[0].Capabilities[0].Id.Should().Be(capability.Id);
            model.CapabilityCategories[0].Capabilities[0].Selected.Should().BeTrue();
            model.CapabilityCategories[0].Capabilities[0].Epics.Should().HaveCount(2);
            model.CapabilityCategories[0].Capabilities[0].Epics.Should().OnlyContain(e => e.Selected);
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
