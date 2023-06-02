using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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

            var model = new AdditionalFiltersModel(frameworks, null, null, string.Empty, string.Empty, string.Empty);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.FrameworkFilter.Should().BeEmpty();

            foreach (var framework in frameworks)
            {
                model.FrameworkOptions.Should().ContainEquivalentOf(new SelectOption<string>
                {
                    Value = framework.Id,
                    Text = $"{framework.ShortName} ({framework.CountOfActiveSolutions})",
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

            var model = new AdditionalFiltersModel(frameworks, framework.Id, null, string.Empty, string.Empty, string.Empty);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);
            model.FrameworkFilter.Should().Be($"{framework.ShortName} ({framework.CountOfActiveSolutions})");
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_With_Expired_Frameworks_And_SelectedFrameworkId_CreatesFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            frameworks.ForEach(f => f.Expired = true);

            var framework = frameworks.Last();

            var model = new AdditionalFiltersModel(frameworks, framework.Id, null, string.Empty, string.Empty, string.Empty);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().BeEmpty();
            model.FrameworkFilter.Should().Be($"{framework.ShortName} ({framework.CountOfActiveSolutions})");
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2 })]
        public static void Constructor_WithClientApplicationTypeSelected_CreatesClientApplicationTypeCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((ClientApplicationType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty, string.Empty);

            model.ClientApplicationTypeOptions.Should().NotBeNull();
            model.ClientApplicationTypeOptions.Should().HaveCount(expectedCount);
            model.ClientApplicationTypeFilters.Should().HaveCount(expectedCount);
            model.ClientApplicationTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.ClientApplicationTypeOptions)
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
        public static void Constructor_WithClientApplicationTypeSelected_CreatesClientApplicationTypeCheckBoxItems_WhenOneSelectedValueIsNotAsExpected(int[] expectedSelectedValues)
        {
            var expectedCount = 3;
            var expectedFilters = expectedSelectedValues.Select(i => ((ClientApplicationType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty, string.Empty);

            model.ClientApplicationTypeOptions.Should().NotBeNull();
            model.ClientApplicationTypeOptions.Should().HaveCount(expectedCount);
            model.ClientApplicationTypeFilters.Should().BeEquivalentTo(expectedFilters);

            foreach (var item in model.ClientApplicationTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [InlineData(null)]
        public static void Constructor_WithNullClientApplicationTypeSelected_CreatesClientApplicationTypeCheckBoxItems_NoneSelected(string selectedClientApplicationTypeIds)
        {
            var expectedCount = 3;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), string.Empty, selectedClientApplicationTypeIds, string.Empty, string.Empty, string.Empty);

            model.ClientApplicationTypeOptions.Should().NotBeNull();
            model.ClientApplicationTypeOptions.Should().HaveCount(expectedCount);

            foreach (var item in model.ClientApplicationTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2, 3 })]
        public static void Constructor_With_SelectedHostingTypeIds_Creates_ClientApplicationTypeCheckBoxItems_AllSelected(int[] expectedSelectedValues)
        {
            var expectedCount = 4;
            var expectedFilters = expectedSelectedValues.Select(i => ((HostingType)i).Name());

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, expectedSelectedValues.ToFilterString(), string.Empty, string.Empty);

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

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, string.Empty, selectedHostingTypeIds, string.Empty, string.Empty);

            model.HostingTypeOptions.Should().NotBeNull();
            model.HostingTypeOptions.Should().HaveCount(expectedCount);
            model.HostingTypeFilters.Should().BeEquivalentTo(Array.Empty<string>());

            foreach (var item in model.HostingTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedCapabilityAndEpics_PropertiesSetCorrectly(string selectedCapabilityIds, string selectedEpicIds)
        {
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, null, null, selectedCapabilityIds, selectedEpicIds);

            model.SelectedCapabilityIds.Should().Be(selectedCapabilityIds);
            model.SelectedEpicIds.Should().Be(selectedEpicIds);
        }
    }
}
