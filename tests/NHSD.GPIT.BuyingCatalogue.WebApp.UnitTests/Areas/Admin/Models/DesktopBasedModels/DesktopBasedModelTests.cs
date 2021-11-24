using System;
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
        public static void FromCatalogueItem_ValidCatalogueItem_ApplicationTypeSetCorrectly(
            CatalogueItem catalogueItem)
        {
            var actual = new DesktopBasedModel(catalogueItem);

            actual.ApplicationType.Should().Equals(ClientApplicationType.Desktop);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DesktopBasedModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
