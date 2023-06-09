using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class FilterCapabilitiesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var model = new FilterCapabilitiesModel(capabilities);

            model.Groups.Should().BeEquivalentTo(capabilities.Select(x => x.Category));
            model.Total.Should().Be(capabilities.Count);
            model.SelectedItems.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = false,
            }));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedIds_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var selectedIds = new[] { capabilities.First().Id, capabilities.Last().Id }.ToFilterString();

            var model = new FilterCapabilitiesModel(capabilities, selectedIds);

            model.Groups.Should().BeEquivalentTo(capabilities.Select(x => x.Category));
            model.Total.Should().Be(capabilities.Count);
            model.SelectedItems.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
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
            var model = new FilterCapabilitiesModel(capabilities);

            foreach (var category in capabilities.Select(x => x.Category))
            {
                var expected = capabilities.Where(x => x.Category.Id == category.Id);

                model.Items(category.Id).Should().BeEquivalentTo(expected);
            }
        }
    }
}
