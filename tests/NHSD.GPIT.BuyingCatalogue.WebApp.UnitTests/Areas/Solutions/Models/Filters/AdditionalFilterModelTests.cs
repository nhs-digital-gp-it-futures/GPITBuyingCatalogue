using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
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
            var model = new AdditionalFiltersModel(frameworks, null, string.Empty, string.Empty, string.Empty);

            model.FrameworkOptions.Should().NotBeNull();
            model.FrameworkOptions.Should().HaveCount(frameworks.Count);

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
        [InlineData("0,1,2", new[] { 0, 1, 2 })]
        public static void Constructor_WithClientApplicationTypeSelected_CreatesClientApplicationTypeCheckBoxItems_AllSelected(string selectedClientApplicationTypeIds, int[] expectedSelectedValues)
        {
            var expectedCount = 3;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), selectedClientApplicationTypeIds, string.Empty, string.Empty, string.Empty);

            model.ClientApplicationTypeOptions.Should().NotBeNull();
            model.ClientApplicationTypeOptions.Should().HaveCount(expectedCount);

            foreach (var item in model.ClientApplicationTypeOptions)
            {
                var isSelected = expectedSelectedValues.Contains(item.Value);
                item.Selected.Should().Be(isSelected);
            }
        }

        [Theory]
        [InlineData("0", new[] { 0 })]
        [InlineData("1", new[] { 1 })]
        [InlineData("2", new[] { 2 })]
        [InlineData("0,1", new[] { 0, 1 })]
        [InlineData("0,2", new[] { 0, 2 })]
        [InlineData("1,2", new[] { 1, 2 })]
        public static void Constructor_WithClientApplicationTypeSelected_CreatesClientApplicationTypeCheckBoxItems_WhenOneSelectedValueIsNotAsExpected(string selectedClientApplicationTypeIds, int[] expectedSelectedValues)
        {
            var expectedCount = 3;

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), selectedClientApplicationTypeIds, string.Empty, string.Empty, string.Empty);

            model.ClientApplicationTypeOptions.Should().NotBeNull();
            model.ClientApplicationTypeOptions.Should().HaveCount(expectedCount);

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

            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), selectedClientApplicationTypeIds, string.Empty, string.Empty, string.Empty);

            model.ClientApplicationTypeOptions.Should().NotBeNull();
            model.ClientApplicationTypeOptions.Should().HaveCount(expectedCount);

            foreach (var item in model.ClientApplicationTypeOptions)
            {
                item.Selected.Should().BeFalse();
            }
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithSelectedCapabilityAndEpics_PropertiesSetCorrectly(string selectedCapabilityIds, string selectedEpicIds)
        {
            var model = new AdditionalFiltersModel(new List<FrameworkFilterInfo>(), null, selectedCapabilityIds, selectedEpicIds, string.Empty);

            model.SelectedCapabilityIds.Should().Be(selectedCapabilityIds);
            model.SelectedEpicIds.Should().Be(selectedEpicIds);
        }
    }
}
