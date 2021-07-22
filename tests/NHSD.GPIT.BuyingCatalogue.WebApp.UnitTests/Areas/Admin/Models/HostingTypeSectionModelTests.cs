using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class HostingTypeSectionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem,
            HostingTypeSectionModel expected)
        {
            expected.SolutionId = catalogueItem.CatalogueItemId;
            expected.SolutionName = catalogueItem.Name;

            var hosting = catalogueItem.Solution?.GetHosting();

            expected.PublicCloud = new PublicCloudModel(hosting?.PublicCloud);
            expected.PrivateCloud = new PrivateCloudModel(hosting?.PrivateCloud);
            expected.Hybrid = new HybridModel(hosting?.HybridHostingType);
            expected.OnPremise = new OnPremiseModel(hosting?.OnPremise);

            var actual = new HostingTypeSectionModel(catalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
            actual.PublicCloud.Should().BeEquivalentTo(expected.PublicCloud);
            actual.PrivateCloud.Should().BeEquivalentTo(expected.PrivateCloud);
            actual.Hybrid.Should().BeEquivalentTo(expected.Hybrid);
            actual.OnPremise.Should().BeEquivalentTo(expected.OnPremise);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new HostingTypeSectionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [AutoData]
        public static void StatusHostingType_CloudTypeAdded_ReturnsCompleted(string summary)
        {
            var model = new HostingTypeSectionModel { PublicCloud = new PublicCloudModel() };
            model.PublicCloud.Summary = summary;

            var actual = model.StatusHostingType();

            actual.Should().Be(FeatureCompletionStatus.Completed);
        }

        [Fact]
        public static void StatusHostingType_NoCloudTypeAdded_ReturnsNotStarted()
        {
            var model = new HostingTypeSectionModel();

            var actual = model.StatusHostingType();

            actual.Should().Be(FeatureCompletionStatus.NotStarted);
        }
    }
}
