using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class AdditionalFilterModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_WithFrameworks_CreatesFrameworkOptions(
            List<FrameworkFilterInfo> frameworks,
            RequestedFilters filters)
        {
            frameworks.ForEach(f => f.Expired = false);

            var model = new AdditionalFiltersModel(frameworks, filters);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.FrameworkFilter.Should().BeEmpty();
            model.FoundationCapabilitiesFilterString.Should().Be("5|11|12|13|14|15|");

            foreach (var framework in frameworks)
            {
                model.FrameworkOptions.Should().ContainEquivalentOf(new SelectOption<string>
                {
                    Value = framework.Id,
                    Text = $"{framework.ShortName}",
                    Selected = false,
                });
            }
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_With_Active_Frameworks_And_SelectedFrameworkId_CreatesFrameworkOptions(
            List<FrameworkFilterInfo> frameworks,
            RequestedFilters filters)
        {
            frameworks.ForEach(f => f.Expired = false);

            var framework = frameworks.Last();
            filters = filters with { SelectedFrameworkId = framework.Id };

            var model = new AdditionalFiltersModel(frameworks, filters);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.FrameworkFilter.Should().Be($"{framework.ShortName}");
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_With_Expired_Frameworks_And_SelectedFrameworkId_CreatesFrameworkOptions(
            List<FrameworkFilterInfo> frameworks,
            RequestedFilters filters)
        {
            frameworks.ForEach(f => f.Expired = true);

            var framework = frameworks.Last();
            filters = filters with { SelectedFrameworkId = framework.Id };

            var model = new AdditionalFiltersModel(frameworks, filters);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().BeEmpty();
            model.FrameworkFilter.Should().Be($"{framework.ShortName}");
        }

        [Fact]
        public static void DefaultConstructor_SetsPropertiesToDefaultValues()
        {
            var model = new AdditionalFiltersModel();

            model.ApplicationTypeFilters.Should().NotBeNull().And.BeEmpty();
            model.HostingTypeFilters.Should().NotBeNull().And.BeEmpty();
            model.InteroperabilityFilters.Should().NotBeNull().And.BeEmpty();
            model.IM1IntegrationsFilters.Should().NotBeNull().And.BeEmpty();
            model.GPConnectIntegrationsFilters.Should().NotBeNull().And.BeEmpty();
            model.CapabilitiesCount.Should().Be(0);
            model.EpicsCount.Should().Be(0);
            model.FoundationCapabilitiesFilterString.Should().Be("5|11|12|13|14|15|");
        }

        [Theory]
        [CommonInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_WithApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((ApplicationType)i).Name());

            filters = filters with { SelectedApplicationTypeIds = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(expectedCount);
            model.ApplicationTypeFilters.Should().HaveCount(expectedCount);
            model.ApplicationTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.ApplicationTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [CommonInlineAutoData(new[] { 0 })]
        [CommonInlineAutoData(new[] { 1 })]
        [CommonInlineAutoData(new[] { 2 })]
        [CommonInlineAutoData(new[] { 0, 1 })]
        [CommonInlineAutoData(new[] { 0, 2 })]
        [CommonInlineAutoData(new[] { 1, 2 })]
        public static void Constructor_WithApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_WhenOneSelectedValueIsNotAsExpected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((ApplicationType)i).Name());

            filters = filters with { SelectedApplicationTypeIds = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(expectedCount);
            model.ApplicationTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.ApplicationTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Constructor_WithNullApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_NoneSelected(
            string selectedApplicationTypeIds,
            RequestedFilters filters)
        {
            var expectedCount = 3;

            filters = filters with { SelectedApplicationTypeIds = selectedApplicationTypeIds };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(expectedCount);

            foreach (var item in model.ApplicationTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonInlineAutoData(new[] { 0, 1, 2, 3 })]
        public static void Constructor_With_SelectedHostingTypeIds_Creates_ApplicationTypeCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedCount = 4;
            var expectedFilters = expectedSelectedValues.Select(i => ((HostingType)i).Name());

            filters = filters with { SelectedHostingTypeIds = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.HostingTypeOptions.Should().NotBeNull();
            model.HostingTypeOptions.Should().HaveCount(expectedCount);
            model.HostingTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.HostingTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedHostingTypeIds_Creates_HostingTypeOptions_NoneSelected(
            string selectedHostingTypeIds,
            RequestedFilters filters)
        {
            var expectedCount = 4;

            filters = filters with { SelectedHostingTypeIds = selectedHostingTypeIds };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.HostingTypeOptions.Should().NotBeNull();
            model.HostingTypeOptions.Should().HaveCount(expectedCount);
            model.HostingTypeFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.HostingTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedIM1IntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropIm1IntegrationType)i).Name());

            filters = filters with { SelectedIM1Integrations = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.IM1IntegrationsOptions.Should().NotBeNull();
            model.IM1IntegrationsOptions.Should().HaveCount(expectedCount);
            model.IM1IntegrationsFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.IM1IntegrationsOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedIM1IntegrationsOptions_Creates_IM1IntegrationsOption_NoneSelected(
            string selectedIM1Integrations,
            RequestedFilters filters)
        {
            var expectedCount = 3;

            filters = filters with { SelectedIM1Integrations = selectedIM1Integrations };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.IM1IntegrationsOptions.Should().NotBeNull();
            model.IM1IntegrationsOptions.Should().HaveCount(expectedCount);
            model.IM1IntegrationsFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.IM1IntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedGPConnectIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropGpConnectIntegrationType)i).Name());

            filters = filters with { SelectedGPConnectIntegrations = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.GPConnectIntegrationsOptions.Should().NotBeNull();
            model.GPConnectIntegrationsOptions.Should().HaveCount(expectedCount);
            model.GPConnectIntegrationsFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.GPConnectIntegrationsOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedGPConnectIntegrationsOptions_Creates_IGPConnectIntegrationsOption_NoneSelected(
            string selectedGPConnectIntegrations,
            RequestedFilters filters)
        {
            var expectedCount = 3;

            filters = filters with { SelectedGPConnectIntegrations = selectedGPConnectIntegrations };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.GPConnectIntegrationsOptions.Should().NotBeNull();
            model.GPConnectIntegrationsOptions.Should().HaveCount(expectedCount);
            model.GPConnectIntegrationsFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.GPConnectIntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonInlineAutoData(new[] { 0, 1 })]
        public static void Constructor_With_SelectedIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedCount = 2;
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropIntegrationType)i).EnumMemberName());

            filters = filters with { SelectedInteroperabilityOptions = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.InteroperabilityOptions.Should().NotBeNull();
            model.InteroperabilityOptions.Should().HaveCount(expectedCount);
            model.InteroperabilityFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.InteroperabilityOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [CommonInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedIntegrationsOptions_Creates_IntegrationsOption_NoneSelected(
            string selectedIntegrations,
            RequestedFilters filters)
        {
            var expectedCount = 2;

            filters = filters with { SelectedInteroperabilityOptions = selectedIntegrations };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.InteroperabilityOptions.Should().NotBeNull();
            model.InteroperabilityOptions.Should().HaveCount(expectedCount);
            model.InteroperabilityFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.InteroperabilityOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonAutoData]
        public static void ToRequestedFilters_With_SelectedIM1Integrations(
            RequestedFilters filters)
        {
            filters = filters with { SelectedInteroperabilityOptions = "0" };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            var requestedFilters = model.ToRequestedFilters();

            requestedFilters.SelectedIM1Integrations.Should().NotBeNull();
            requestedFilters.SelectedGPConnectIntegrations.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void ToRequestedFilters_With_SelectedGPConnectIntegrations(
            RequestedFilters filters)
        {
            filters = filters with { SelectedInteroperabilityOptions = "1" };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            var requestedFilters = model.ToRequestedFilters();

            requestedFilters.SelectedIM1Integrations.Should().BeNull();
            requestedFilters.SelectedGPConnectIntegrations.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedCapabilityAndEpics_Property_SetCorrectly(
            string selected,
            RequestedFilters filters)
        {
            filters = filters with { Selected = selected };
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), filters);

            model.Selected.Should().Be(selected);
        }
    }
}
