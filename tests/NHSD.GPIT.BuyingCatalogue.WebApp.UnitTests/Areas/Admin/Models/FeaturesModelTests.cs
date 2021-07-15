using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags.NhsTagsTagHelper;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class FeaturesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void AllFeatures_Returns_ValidFeatures(FeaturesModel model)
        {
            var expected = new[]
                {
                    model.Feature01,
                    model.Feature02,
                    model.Feature03,
                    model.Feature04,
                    model.Feature05,
                    model.Feature06,
                    model.Feature07,
                    model.Feature08,
                    model.Feature09,
                    model.Feature10,
                }.Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();

            var actual = model.AllFeatures;

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("Feature01")]
        [InlineData("Feature02")]
        [InlineData("Feature03")]
        [InlineData("Feature04")]
        [InlineData("Feature05")]
        [InlineData("Feature06")]
        [InlineData("Feature07")]
        [InlineData("Feature08")]
        [InlineData("Feature09")]
        [InlineData("Feature10")]
        public static void FeatureProperties_StringLength_SetTo100(string property)
        {
            typeof(FeaturesModel)
                .GetProperty(property, BindingFlags.Instance | BindingFlags.Public)
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength
                .Should()
                .Be(100);
        }

        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem,
            FeaturesModel expected)
        {
            catalogueItem.Solution.Features = JsonConvert.SerializeObject(expected.AllFeatures);
            expected.SolutionId = catalogueItem.CatalogueItemId;
            expected.SolutionName = catalogueItem.Name;

            var actual = new FeaturesModel().FromCatalogueItem(catalogueItem);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new FeaturesModel().FromCatalogueItem(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [AutoData]
        public static void StatusFeatures_OneFeatureAdded_ReturnsCompleted(string feature)
        {
            var model = new FeaturesModel { Feature01 = feature };

            var actual = model.StatusFeatures();

            actual.Should().Be("Completed");
        }

        [Fact]
        public static void StatusFeatures_NoFeatureAdded_ReturnsNotStarted()
        {
            var model = new FeaturesModel { Feature01 = null, Feature05 = string.Empty, Feature10 = "    " };

            var actual = model.StatusFeatures();

            actual.Should().Be("Not started");
        }

        [Theory]
        [AutoData]
        public static void StatusFeaturesColor_OneFeatureAdded_ReturnsCompleted(string feature)
        {
            var model = new FeaturesModel { Feature01 = feature };

            var actual = model.StatusFeaturesColor();

            actual.Should().Be(TagColour.Green);
        }

        [Fact]
        public static void StatusFeaturesColor_NoFeatureAdded_ReturnsNotStarted()
        {
            var model = new FeaturesModel { Feature01 = null, Feature05 = string.Empty, Feature10 = "    " };

            var actual = model.StatusFeaturesColor();

            actual.Should().Be(TagColour.Grey);
        }
    }
}
