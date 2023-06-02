using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class FilterEpicsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            List<Capability> capabilities,
            List<Epic> epics)
        {
            for (var i = 0; i < capabilities.Count; i++)
            {
                epics[i].Capability = capabilities[i];
            }

            var model = new FilterEpicsModel(capabilities, epics);

            model.CapabilityIds.Should().Be(capabilities.Select(x => x.Id).ToFilterString());
            model.Groups.Should().BeEquivalentTo(capabilities);
            model.Total.Should().Be(epics.Count);
            model.SelectedItems.Should().BeEquivalentTo(epics.Select(x => new SelectionModel
            {
                Id = x.Id,
                Selected = false,
            }));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedIds_PropertiesAreSetCorrectly(
            List<Capability> capabilities,
            List<Epic> epics)
        {
            for (var i = 0; i < capabilities.Count; i++)
            {
                epics[i].Capability = capabilities[i];
            }

            var selectedIds = new[] { epics.First().Id, epics.Last().Id }.ToFilterString();
            var model = new FilterEpicsModel(capabilities, epics, selectedIds);

            model.CapabilityIds.Should().Be(capabilities.Select(x => x.Id).ToFilterString());
            model.Groups.Should().BeEquivalentTo(capabilities);
            model.Total.Should().Be(epics.Count);
            model.SelectedItems.Should().BeEquivalentTo(epics.Select(x => new SelectionModel
            {
                Id = x.Id,
                Selected = x.Id == epics.First().Id || x.Id == epics.Last().Id,
            }));
        }

        [Theory]
        [CommonAutoData]
        public static void Items_ReturnsExpectedResult(
            List<Capability> capabilities,
            List<Epic> epics)
        {
            for (var i = 0; i < capabilities.Count; i++)
            {
                epics[i].Capability = capabilities[i];
            }

            var model = new FilterEpicsModel(capabilities, epics);

            foreach (var capability in capabilities)
            {
                var expected = epics.Where(x => x.Capability.Id == capability.Id);

                model.Items(capability.Id).Should().BeEquivalentTo(expected);
            }
        }
    }
}
