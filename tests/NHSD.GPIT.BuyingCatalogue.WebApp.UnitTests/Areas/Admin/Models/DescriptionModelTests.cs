using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags.NhsTagsTagHelper;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class DescriptionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem,
            DescriptionModel expected)
        {
            expected.Summary = catalogueItem.Solution?.Summary;
            expected.Description = catalogueItem.Solution?.FullDescription;
            expected.Link = catalogueItem.Solution.AboutUrl;
            expected.SolutionName = catalogueItem?.Name;

            var actual = new DescriptionModel(catalogueItem);

            actual.Summary.Should().BeEquivalentTo(expected.Summary);
            actual.Description.Should().BeEquivalentTo(expected.Description);
            actual.Link.Should().BeEquivalentTo(expected.Link);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DescriptionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [AutoData]
        public static void StatusDescription_SummaryAdded_ReturnsCompleted(string summary)
        {
            var model = new DescriptionModel { Summary = summary };

            var actual = model.StatusDescription();

            actual.Should().Be(FeatureCompletionStatus.Completed);
        }

        [Fact]
        public static void StatusDescription_NoSummaryAdded_ReturnsNotStarted()
        {
            var model = new DescriptionModel { Summary = null };

            var actual = model.StatusDescription();

            actual.Should().Be(FeatureCompletionStatus.NotStarted);
        }
    }
}
