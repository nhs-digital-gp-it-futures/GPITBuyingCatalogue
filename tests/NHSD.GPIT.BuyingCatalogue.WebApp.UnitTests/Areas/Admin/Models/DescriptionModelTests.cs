using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class DescriptionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution,
            DescriptionModel expected)
        {
            var catalogueItem = solution.CatalogueItem;

            expected.Summary = solution.Summary;
            expected.Description = solution.FullDescription;
            expected.Link = solution.AboutUrl;
            expected.SolutionName = catalogueItem.Name;

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

            var actual = model.Status();

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void StatusDescription_NoSummaryAdded_ReturnsNotStarted(string invalid)
        {
            var model = new DescriptionModel { Summary = invalid };

            var actual = model.Status();

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void StatusDescription_LinkAdded_ReturnsInProgress(string link)
        {
            var model = new DescriptionModel { Link = link };

            var actual = model.Status();

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [AutoData]
        public static void StatusDescription_DescriptionAdded_ReturnsInProgress(string description)
        {
            var model = new DescriptionModel { Description = description };

            var actual = model.Status();

            actual.Should().Be(TaskProgress.InProgress);
        }
    }
}
