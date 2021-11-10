using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels
{
    public static class InteroperabilityModelTests
    {
        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new InteroperabilityModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution,
            InteroperabilityModel expected)
        {
            var catalogueItem = solution.CatalogueItem;
            expected.SolutionName = catalogueItem.Name;
            expected.Link = solution.IntegrationsUrl;

            var actual = new InteroperabilityModel(catalogueItem);

            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}");
            actual.CatalogueItemId.Should().Be(catalogueItem.Id);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
            actual.Link.Should().BeEquivalentTo(expected.Link);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.Status().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_NoIntegrations_ShouldBeOptional(
            InteroperabilityModel model)
        {
            model.Link = null;
            model.IM1Integrations = Array.Empty<Integration>();
            model.GpConnectIntegrations = Array.Empty<Integration>();

            model.Status().Should().Be(TaskProgress.Optional);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_LinkOnly_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.IM1Integrations = Array.Empty<Integration>();
            model.GpConnectIntegrations = Array.Empty<Integration>();

            model.Status().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_IM1IntegrationsOnly_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.Link = null;
            model.GpConnectIntegrations = Array.Empty<Integration>();

            model.Status().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_GpConnectIntegrationsOnly_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.Link = null;
            model.IM1Integrations = Array.Empty<Integration>();

            model.Status().Should().Be(TaskProgress.Completed);
        }
    }
}
