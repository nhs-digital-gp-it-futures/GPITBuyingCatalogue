using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class RoadmapModelTests
    {
        [Fact]
        public static void Link_Should_BeCorrectlyDecorated()
        {
            var propertyInfo = typeof(RoadmapModel)
                .GetProperty(nameof(RoadmapModel.Link), BindingFlags.Instance | BindingFlags.Public);

            propertyInfo
                .Should()
                .BeDecoratedWith<StringLengthAttribute>(s => s.MaximumLength == 1000);
            propertyInfo
                .Should()
                .BeDecoratedWith<UrlAttribute>();
        }

        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem,
            RoadmapModel expected)
        {
            expected.Link = catalogueItem.Solution.RoadMap;
            expected.SolutionId = catalogueItem.CatalogueItemId;
            expected.SolutionName = catalogueItem.Name;

            var actual = new RoadmapModel().FromCatalogueItem(catalogueItem);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new RoadmapModel().FromCatalogueItem(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [AutoData]
        public static void Status_LinkAdded_ReturnsCompleted(string link)
        {
            var model = new RoadmapModel { Link = link };

            var actual = model.Status();

            actual.Should().Be(FeatureCompletionStatus.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_LinkInvalid_ReturnsNotStarted(string invalid)
        {
            var model = new RoadmapModel { Link = invalid };

            var actual = model.Status();

            actual.Should().Be(FeatureCompletionStatus.NotStarted);
        }
    }
}
