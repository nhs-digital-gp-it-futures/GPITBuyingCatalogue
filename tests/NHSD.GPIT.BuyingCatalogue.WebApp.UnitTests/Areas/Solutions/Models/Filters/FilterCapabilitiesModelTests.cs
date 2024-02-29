using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class FilterCapabilitiesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_IsFilter_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var model = new FilterCapabilitiesModel(capabilities, null)
            {
                IsFilter = true,
            };

            model.CapabilityGroups.Should().BeEquivalentTo(capabilities.Select(x => new { x.Category.Id, x.Category.Name }));
            model.CapabilitySelectionItems.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = false,
            }));
            model.IsFilter.Should().BeTrue();
            model.GetPageTitle().Should().BeEquivalentTo(FilterCapabilitiesModel.FilterPageTitle);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NotFilter_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var model = new FilterCapabilitiesModel(capabilities, null);

            model.CapabilityGroups.Should().BeEquivalentTo(capabilities.Select(x => new { x.Category.Id, x.Category.Name }));
            model.CapabilitySelectionItems.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = false,
            }));
            model.IsFilter.Should().BeFalse();
            model.GetPageTitle().Should().BeEquivalentTo(FilterCapabilitiesModel.SupplierDefinedEpicPageTitle);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedIds_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var selected = new Dictionary<int, string[]>(new[]
            {
                new KeyValuePair<int, string[]>(capabilities.First().Id, System.Array.Empty<string>()),
                new KeyValuePair<int, string[]>(capabilities.Last().Id, System.Array.Empty<string>()),
            });

            var model = new FilterCapabilitiesModel(capabilities, selected.Keys)
            {
                IsFilter = true,
            };

            model.CapabilityGroups.Should().BeEquivalentTo(capabilities.Select(x => new { x.Category.Id, x.Category.Name }));
            model.CapabilitySelectionItems.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = x.Id == capabilities.First().Id || x.Id == capabilities.Last().Id,
            }));
        }

        [Theory]
        [CommonAutoData]
        public static void Capabilities_ReturnsExpectedResult(
            List<Capability> capabilities)
        {
            var model = new FilterCapabilitiesModel(capabilities, null)
            {
                IsFilter = true,
            };

            foreach (var category in capabilities.Select(x => x.Category))
            {
                var expected = capabilities
                    .Where(x => x.Category.Id == category.Id)
                    .OrderBy(x => x.Name)
                    .Select(x => x.Name);

                model.Items(category.Id).Should().BeEquivalentTo(expected);
            }
        }
    }
}
