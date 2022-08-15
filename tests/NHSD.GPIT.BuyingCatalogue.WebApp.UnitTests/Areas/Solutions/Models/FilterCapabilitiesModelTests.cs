using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class FilterCapabilitiesModelTests
    {
        [Fact]
        public static void Constructor_NullCapabilities_ThrowsException()
        {
            FluentActions
                .Invoking(() => new FilterCapabilitiesModel(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var model = new FilterCapabilitiesModel(capabilities);

            model.Categories.Should().BeEquivalentTo(capabilities.Select(x => x.Category));
            model.TotalCapabilities.Should().Be(capabilities.Count);
            model.Items.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
            {
                Id = x.Id,
                Selected = false,
            }));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedIds_PropertiesAreSetCorrectly(
            List<Capability> capabilities)
        {
            var selectedIds = $"{capabilities.First().Id}{FilterCapabilitiesModel.FilterDelimiter}{capabilities.Last().Id}";

            var model = new FilterCapabilitiesModel(capabilities, selectedIds);

            model.Categories.Should().BeEquivalentTo(capabilities.Select(x => x.Category));
            model.TotalCapabilities.Should().Be(capabilities.Count);
            model.Items.Should().BeEquivalentTo(capabilities.Select(x => new SelectionModel
            {
                Id = x.Id,
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

                model.Capabilities(category.Id).Should().BeEquivalentTo(expected);
            }
        }
    }
}
