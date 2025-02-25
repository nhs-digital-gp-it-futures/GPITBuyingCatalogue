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

        [Theory]
        [MockAutoData]
        public static void GetIntegrationIds_Accepts_Inner_Selections(
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            RequestedFilters filters,
            List<Integration> integrations,
            List<IntegrationType> integrationTypes)
        {
            integrations.ForEach(
                x => x.IntegrationTypes = integrationTypes.Where(y => y.IntegrationId == x.Id).ToList());

            Integration selectedIntegration = integrations[1];
            IntegrationType selectedIntegrationType = selectedIntegration.IntegrationTypes.First();

            var integrationSelectionString = $"{selectedIntegration.Id}.{selectedIntegrationType.Id}|";
            RequestedFilters newFilters = filters with { SelectedIntegrations = integrationSelectionString };

            var model = new AdditionalFiltersModel(frameworks, newFilters, integrations);
            model.IntegrationOptions.ForEach(x => x.Selected = false);

            var expectedSelectionIds =
                $"{(int)selectedIntegration.Id}.{selectedIntegrationType.Id}|"; // output is enum value instead of enum name

            var actualSelectionIds = model.GetIntegrationIds();

            Assert.Equal(expectedSelectionIds, actualSelectionIds);
        }
    }
}
