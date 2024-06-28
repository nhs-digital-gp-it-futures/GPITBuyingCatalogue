using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.Filters
{
    public static class AdditionalFilterModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructor_WithFrameworks_CreatesFrameworkOptions(
            List<FrameworkFilterInfo> frameworks,
            RequestedFilters filters)
        {
            frameworks.ForEach(f => f.Expired = false);

            var model = new AdditionalFiltersModel(frameworks, filters);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.SelectedFrameworkId.Should().Be(filters.SelectedFrameworkId);

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
        [MockAutoData]
        public static void Constructor_With_Expired_Frameworks_CreatesFrameworkOptions(
            List<FrameworkFilterInfo> frameworks,
            RequestedFilters filters)
        {
            frameworks.ForEach(f => f.Expired = true);

            var model = new AdditionalFiltersModel(frameworks, filters);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.SelectedFrameworkId.Should().Be(filters.SelectedFrameworkId);

            foreach (var framework in frameworks)
            {
                model.FrameworkOptions.Should().ContainEquivalentOf(new SelectOption<string>
                {
                    Value = framework.Id,
                    Text = $"{framework.ShortName} (expired)",
                    Selected = false,
                });
            }
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
            model.NhsAppIntegrationsFilters.Should().NotBeNull().And.BeEmpty();
            model.CapabilitiesCount.Should().Be(0);
            model.EpicsCount.Should().Be(0);
            model.FoundationCapabilitiesFilterString.Should().Be("5|11|12|13|14|15|");
        }

        [Theory]
        [MockInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_WithApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((ApplicationType)i).Name());

            filters = filters with { SelectedApplicationTypeIds = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(3);
            model.ApplicationTypeFilters.Should().HaveCount(3);
            model.ApplicationTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.ApplicationTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(new[] { 0 })]
        [MockInlineAutoData(new[] { 1 })]
        [MockInlineAutoData(new[] { 2 })]
        [MockInlineAutoData(new[] { 0, 1 })]
        [MockInlineAutoData(new[] { 0, 2 })]
        [MockInlineAutoData(new[] { 1, 2 })]
        public static void Constructor_WithApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_WhenOneSelectedValueIsNotAsExpected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((ApplicationType)i).Name());

            filters = filters with { SelectedApplicationTypeIds = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(3);
            model.ApplicationTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.ApplicationTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(null)]
        public static void Constructor_WithNullApplicationTypeSelected_CreatesApplicationTypeCheckBoxItems_NoneSelected(
            string selectedApplicationTypeIds,
            RequestedFilters filters)
        {
            filters = filters with { SelectedApplicationTypeIds = selectedApplicationTypeIds };
            var model = new AdditionalFiltersModel([], filters);

            model.ApplicationTypeOptions.Should().NotBeNull();
            model.ApplicationTypeOptions.Should().HaveCount(3);

            foreach (var item in model.ApplicationTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [MockInlineAutoData(new[] { 0, 1, 2, 3 })]
        public static void Constructor_With_SelectedHostingTypeIds_Creates_ApplicationTypeCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((HostingType)i).Name());

            filters = filters with { SelectedHostingTypeIds = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.HostingTypeOptions.Should().NotBeNull();
            model.HostingTypeOptions.Should().HaveCount(4);
            model.HostingTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.HostingTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedHostingTypeIds_Creates_HostingTypeOptions_NoneSelected(
            string selectedHostingTypeIds,
            RequestedFilters filters)
        {
            filters = filters with { SelectedHostingTypeIds = selectedHostingTypeIds };
            var model = new AdditionalFiltersModel([], filters);

            model.HostingTypeOptions.Should().NotBeNull();
            model.HostingTypeOptions.Should().HaveCount(4);
            model.HostingTypeFilters.Should().BeEquivalentTo([]);

            foreach (var item in model.HostingTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [MockInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedIM1IntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropIm1IntegrationType)i).Name());

            filters = filters with { SelectedIM1Integrations = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.IM1IntegrationsOptions.Should().NotBeNull();
            model.IM1IntegrationsOptions.Should().HaveCount(3);
            model.IM1IntegrationsFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.IM1IntegrationsOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedIM1IntegrationsOptions_Creates_IM1IntegrationsOption_NoneSelected(
            string selectedIM1Integrations,
            RequestedFilters filters)
        {
            filters = filters with { SelectedIM1Integrations = selectedIM1Integrations };
            var model = new AdditionalFiltersModel([], filters);

            model.IM1IntegrationsOptions.Should().NotBeNull();
            model.IM1IntegrationsOptions.Should().HaveCount(3);
            model.IM1IntegrationsFilters.Should().BeEquivalentTo([]);

            foreach (var item in model.IM1IntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [MockInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedGPConnectIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropGpConnectIntegrationType)i).Name());

            filters = filters with { SelectedGPConnectIntegrations = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.GPConnectIntegrationsOptions.Should().NotBeNull();
            model.GPConnectIntegrationsOptions.Should().HaveCount(5);
            model.GPConnectIntegrationsFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.GPConnectIntegrationsOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedGPConnectIntegrationsOptions_Creates_IGPConnectIntegrationsOption_NoneSelected(
            string selectedGPConnectIntegrations,
            RequestedFilters filters)
        {
            filters = filters with { SelectedGPConnectIntegrations = selectedGPConnectIntegrations };
            var model = new AdditionalFiltersModel([], filters);

            model.GPConnectIntegrationsOptions.Should().NotBeNull();
            model.GPConnectIntegrationsOptions.Should().HaveCount(5);
            model.GPConnectIntegrationsFilters.Should().BeEquivalentTo([]);

            foreach (var item in model.GPConnectIntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [MockInlineAutoData(new[] { 0, 1, 2, 3 })]
        public static void Constructor_With_SelectedNhsAppIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((InteropNhsAppIntegrationType)i).Name());

            filters = filters with { SelectedNhsAppIntegrations = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.NhsAppIntegrationsOptions.Should().NotBeNull();
            model.NhsAppIntegrationsOptions.Should().HaveCount(4);
            model.NhsAppIntegrationsFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.NhsAppIntegrationsOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedNHSAppIntegrationsOptions_Creates_NHSAppIntegrationsOption_NoneSelected(
            string selectedNhsAppIntegrations,
            RequestedFilters filters)
        {
            filters = filters with { SelectedNhsAppIntegrations = selectedNhsAppIntegrations };
            var model = new AdditionalFiltersModel([], filters);

            model.NhsAppIntegrationsOptions.Should().NotBeNull();
            model.NhsAppIntegrationsOptions.Should().HaveCount(4);
            model.NhsAppIntegrationsFilters.Should().BeEquivalentTo([]);

            foreach (var item in model.NhsAppIntegrationsOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [MockInlineAutoData(new[] { 0, 1, 2 })]
        public static void Constructor_With_SelectedIntegrationsOptions_Creates_InteroperabilityCheckBoxItems_AllSelected(
            int[] expectedSelectedValues,
            RequestedFilters filters)
        {
            var expectedFilters = expectedSelectedValues.Select(i => ((SupportedIntegrations)i).EnumMemberName());

            filters = filters with { SelectedInteroperabilityOptions = expectedSelectedValues.ToFilterString() };
            var model = new AdditionalFiltersModel([], filters);

            model.InteroperabilityOptions.Should().NotBeNull();
            model.InteroperabilityOptions.Should().HaveCount(3);
            model.InteroperabilityFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.InteroperabilityOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [MockInlineAutoData(null)]
        public static void Constructor_WithNull_SelectedIntegrationsOptions_Creates_IntegrationsOption_NoneSelected(
            string selectedIntegrations,
            RequestedFilters filters)
        {
            filters = filters with { SelectedInteroperabilityOptions = selectedIntegrations };
            var model = new AdditionalFiltersModel([], filters);

            model.InteroperabilityOptions.Should().NotBeNull();
            model.InteroperabilityOptions.Should().HaveCount(3);
            model.InteroperabilityFilters.Should().BeEquivalentTo([]);

            foreach (var item in model.InteroperabilityOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_WithSelectedCapabilityAndEpics_Property_SetCorrectly(
            string selected,
            RequestedFilters filters)
        {
            filters = filters with { Selected = selected };
            var model = new AdditionalFiltersModel([], filters);

            model.Selected.Should().Be(selected);
        }

        [Theory]
        [MockAutoData]
        public static void SetParentFilters_InteroperabilityOptionsNull_ReturnsWithoutUpdating(
            AdditionalFiltersModel model)
        {
            model.InteroperabilityOptions = null;
            model.SetParentFilters();
            model.InteroperabilityOptions.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void SetParentFilters_ChildFilterNotSet_ParentsNotSet(
            AdditionalFiltersModel model)
        {
            model.InteroperabilityOptions = Enum.GetValues(typeof(SupportedIntegrations))
                .Cast<SupportedIntegrations>()
                .Select(
                    x => new SelectOption<int> { Value = (int)x, Selected = false })
                .ToList();

            model.IM1IntegrationsOptions = null;
            model.GPConnectIntegrationsOptions = null;
            model.NhsAppIntegrationsOptions = null;

            model.SetParentFilters();

            model.InteroperabilityFilters.Length.Should().Be(0);
        }

        [Theory]
        [MockAutoData]
        public static void SetParentFilters_ChildFiltersSet_ParentsSetCorrectly(
            AdditionalFiltersModel model)
        {
            model.InteroperabilityOptions = Enum.GetValues(typeof(SupportedIntegrations))
                .Cast<SupportedIntegrations>()
                .Select(
                    x => new SelectOption<int> { Value = (int)x, Selected = false })
                .ToList();

            model.IM1IntegrationsOptions =
                new List<SelectOption<int>>() { new SelectOption<int>() { Value = 0, Selected = true } };
            model.GPConnectIntegrationsOptions =
                new List<SelectOption<int>>() { new SelectOption<int>() { Value = 0, Selected = true } };
            model.NhsAppIntegrationsOptions =
                new List<SelectOption<int>>() { new SelectOption<int>() { Value = 0, Selected = true } };

            model.SetParentFilters();

            model.InteroperabilityOptions.Should().NotBeNull();
            model.InteroperabilityOptions.First(x => x.Value == (int)SupportedIntegrations.Im1).Should().NotBeNull();
            model.InteroperabilityOptions.First(x => x.Value == (int)SupportedIntegrations.Im1).Selected.Should().Be(true);
            model.InteroperabilityOptions.First(x => x.Value == (int)SupportedIntegrations.GpConnect).Should().NotBeNull();
            model.InteroperabilityOptions.First(x => x.Value == (int)SupportedIntegrations.GpConnect).Selected.Should().Be(true);
            model.InteroperabilityOptions.First(x => x.Value == (int)SupportedIntegrations.NhsApp).Should().NotBeNull();
            model.InteroperabilityOptions.First(x => x.Value == (int)SupportedIntegrations.NhsApp).Selected.Should().Be(true);
        }
    }
}
