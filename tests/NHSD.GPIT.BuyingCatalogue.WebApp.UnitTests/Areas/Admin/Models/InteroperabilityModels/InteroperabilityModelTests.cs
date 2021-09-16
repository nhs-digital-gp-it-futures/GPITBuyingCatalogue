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
            CatalogueItem catalogueItem,
            InteroperabilityModel expected)
        {
            expected.SolutionName = catalogueItem.Name;
            expected.Link = catalogueItem.Solution.IntegrationsUrl;

            var actual = new InteroperabilityModel(catalogueItem);

            actual.BackLinkText.Should().Be("Go back");
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
        public static void Status_NoIntegrations_ShouldBeNotStarted(
            InteroperabilityModel model)
        {
            model.Link = null;
            model.IM1Integrations = new Integration[0];
            model.GpConnectIntegrations = new Integration[0];

            model.Status().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_LinkOnly_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.IM1Integrations = new Integration[0];
            model.GpConnectIntegrations = new Integration[0];

            model.Status().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_IM1IntegrationsOnly_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.Link = null;
            model.GpConnectIntegrations = new Integration[0];

            model.Status().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_GpConnectIntegrationsOnly_ShouldBeComplete(
            InteroperabilityModel model)
        {
            model.Link = null;
            model.IM1Integrations = new Integration[0];

            model.Status().Should().Be(TaskProgress.Completed);
        }
    }
}
