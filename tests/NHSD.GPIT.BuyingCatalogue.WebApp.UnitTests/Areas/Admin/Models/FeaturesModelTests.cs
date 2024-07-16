using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class FeaturesModelTests
    {
        [Theory]
        [MockAutoData]
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
        public static void FeatureProperties_StringLengthAttribute_SetTo100(string property)
        {
            typeof(FeaturesModel)
                .GetProperty(property, BindingFlags.Instance | BindingFlags.Public)
                .Should()
                .BeDecoratedWith<StringLengthAttribute>(s => s.MaximumLength == 100);
        }

        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution,
            FeaturesModel expected)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Features = JsonSerializer.Serialize(expected.AllFeatures);

            expected.SolutionId = catalogueItem.Id;
            expected.SolutionName = catalogueItem.Name;

            var actual = new FeaturesModel(catalogueItem)
            {
                Title = expected.Title,
                Caption = expected.Caption,
                Advice = expected.Advice,
                AdditionalAdvice = expected.AdditionalAdvice,
            };

            actual.Should().BeEquivalentTo(
                expected,
                options => options
                    .Excluding(fm => fm.BackLink)
                    .Excluding(fm => fm.BackLinkText));
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new FeaturesModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
