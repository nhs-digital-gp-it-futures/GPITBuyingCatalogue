using System;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DesktopBasedModels
{
    public static class DesktopBasedModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_NoExistingNativeDesktop_BackLinkSetCorrectly(
            CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes.Clear();
            clientApplication.ClientApplicationTypes.Add("browser-based");
            catalogueItem.Solution.ClientApplication = JsonSerializer.Serialize(clientApplication);

            var actual = new DesktopBasedModel(catalogueItem);
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/add-application-type");
        }

        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_WithNativeDesktop_BackLinkSetCorrectly(
            CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes.Clear();
            clientApplication.ClientApplicationTypes.Add("native-desktop");
            catalogueItem.Solution.ClientApplication = JsonSerializer.Serialize(clientApplication);

            var actual = new DesktopBasedModel(catalogueItem);

            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type");
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DesktopBasedModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
