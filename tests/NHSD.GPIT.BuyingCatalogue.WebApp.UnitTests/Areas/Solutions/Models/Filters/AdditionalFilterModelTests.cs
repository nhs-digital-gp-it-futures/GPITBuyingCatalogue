using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
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
        public static void Constructor_WithFrameworks_CreatesFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            frameworks.ForEach(f => f.Expired = false);

            var model = new AdditionalFiltersModel(frameworks, null, null, string.Empty, string.Empty, null, null, null);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.FrameworkFilter.Should().BeEmpty();

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
        public static void Constructor_With_Active_Frameworks_And_SelectedFrameworkId_CreatesFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            frameworks.ForEach(f => f.Expired = false);

            var framework = frameworks.Last();

            var model = new AdditionalFiltersModel(frameworks, framework.Id, null, string.Empty, string.Empty, null, null, null);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.FrameworkFilter.Should().Be($"{framework.ShortName}");
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_With_Expired_Frameworks_And_SelectedFrameworkId_CreatesFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            frameworks.ForEach(f => f.Expired = true);

            var framework = frameworks.Last();

            var model = new AdditionalFiltersModel(frameworks, framework.Id, null, string.Empty, null, null, null, string.Empty);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().BeEmpty();
            model.FrameworkFilter.Should().Be($"{framework.ShortName}");
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2 })]
        public static void Constructor_WithApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((ApplicationType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, expectedSelectedValues.ToFilterString(), string.Empty, null, null, null, string.Empty);

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
        [InlineData(new[] { 0 })]
        [InlineData(new[] { 1 })]
        [InlineData(new[] { 2 })]
        [InlineData(new[] { 0, 1 })]
        [InlineData(new[] { 0, 2 })]
        [InlineData(new[] { 1, 2 })]
        public static void Constructor_WithApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_WhenOneSelectedValueIsNotAsExpected(int[] expectedSelectedValues)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((ApplicationType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

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
        [InlineData(null)]
        public static void Constructor_WithNullApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_NoneSelected(string selectedApplicationTypeIds)
        {
            var expectedCount = 3;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), string.Empty, selectedApplicationTypeIds, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(expectedCount);

            foreach (var item in model.ApplicationTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2, 3 })]
        public static void Constructor_With_SelectedHostingTypeIds_Creates_ApplicationTypeCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 4;
            var expectedFilters = expectedSelectedValues.Select(i => ((HostingType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty, string.Empty, string.Empty);

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
        [InlineData(null)]
        public static void Constructor_WithNull_SelectedHostingTypeIds_Creates_HostingTypeOptions_NoneSelected(string selectedHostingTypeIds)
        {
            var expectedCount = 4;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, selectedHostingTypeIds, string.Empty, string.Empty, string.Empty, string.Empty);

            model.HostingTypeOptions.Should().NotBeNull();
            model.HostingTypeOptions.Should().HaveCount(expectedCount);
            model.HostingTypeFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.HostingTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedIM1IntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropIm1IntegrationType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, string.Empty, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty, string.Empty);

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
        [InlineData(null)]
        public static void Constructor_WithNull_SelectedIM1IntegrationsOptions_Creates_IM1IntegrationsOption_NoneSelected(string selectedIM1Integrations)
        {
            var expectedCount = 3;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, string.Empty, selectedIM1Integrations, string.Empty, string.Empty, string.Empty);

            model.IM1IntegrationsOptions.Should().NotBeNull();
            model.IM1IntegrationsOptions.Should().HaveCount(expectedCount);
            model.IM1IntegrationsFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.IM1IntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedGPConnectIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropGpConnectIntegrationType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, string.Empty, string.Empty, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty);

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
        [InlineData(null)]
        public static void Constructor_WithNull_SelectedGPConnectIntegrationsOptions_Creates_IGPConnectIntegrationsOption_NoneSelected(string selectedGPConnectIntegrations)
        {
            var expectedCount = 3;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, string.Empty, string.Empty, selectedGPConnectIntegrations, string.Empty, string.Empty);

            model.GPConnectIntegrationsOptions.Should().NotBeNull();
            model.GPConnectIntegrationsOptions.Should().HaveCount(expectedCount);
            model.GPConnectIntegrationsFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.GPConnectIntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(new[] { 0, 1 })]
        public static void Constructor_With_SelectedIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 2;
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropIntegrationType)i).EnumMemberName());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, string.Empty, string.Empty, string.Empty, expectedSelectedValues.ToFilterString(), string.Empty);

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
        [InlineData(null)]
        public static void Constructor_WithNull_SelectedIntegrationsOptions_Creates_IntegrationsOption_NoneSelected(string selectedIntegrations)
        {
            var expectedCount = 2;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, string.Empty, string.Empty, string.Empty, selectedIntegrations, string.Empty);

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
        public static void Constructor_WithSelectedCapabilityAndEpics_Property_SetCorrectly(string selected)
        {
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, null, null, null, null, null, selected);

            model.Selected.Should().Be(selected);
        }
    }
}
