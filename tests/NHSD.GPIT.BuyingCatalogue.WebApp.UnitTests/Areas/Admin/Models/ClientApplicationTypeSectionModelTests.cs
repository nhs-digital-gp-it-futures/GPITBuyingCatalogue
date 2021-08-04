using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ClientApplicationTypeSectionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            [Frozen] CatalogueItem catalogueItem,
            ClientApplicationTypeSectionModel expected)
        {
            var actual = new ClientApplicationTypeSectionModel(catalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ClientApplicationTypeSectionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [CommonAutoData]
        public static void StatusClientApplicationType_AvailableType_ReturnsCompleted(
            ClientApplicationTypeSectionModel model)
        {
            var actual = model.StatusClientApplicationType();

            actual.Should().Be(FeatureCompletionStatus.Completed);
        }

        [Fact]
        public static void StatusClientApplicationType_NoApplicationTypeAdded_ReturnsNotStarted()
        {
            var model = new ClientApplicationTypeSectionModel();

            var actual = model.StatusClientApplicationType();

            actual.Should().Be(FeatureCompletionStatus.NotStarted);
        }
    }
}
