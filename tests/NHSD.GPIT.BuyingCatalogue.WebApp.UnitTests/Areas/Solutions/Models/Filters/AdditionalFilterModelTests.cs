using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class AdditionalFilterModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructor_SetsPropertiesAsExpected(
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            RequestedFilters filters,
            List<Integration> integrations)
        {
            var model = new AdditionalFiltersModel(frameworks, filters, integrations);

            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.ApplicationTypeOptions.Should().HaveCount(Enum.GetValues<ApplicationType>().Length);
            model.HostingTypeOptions.Should().HaveCount(Enum.GetValues<HostingType>().Length);

            model.Selected.Should().Be(filters.Selected);
            model.SelectedFrameworkId.Should().BeEquivalentTo(filters.SelectedFrameworkId);
            model.SortBy.Should().Be(filters.SortBy);

            model.IntegrationOptions.Should().HaveCount(integrations.Count);
            model.CapabilitiesCount.Should().Be(filters.GetCapabilityAndEpicIds().Count);
            model.EpicsCount.Should().Be(filters.GetCapabilityAndEpicIds().Values.Sum(v => v.Length));
        }
    }
}
