using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
{
    public static class MobileTabletBasedModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_ApplicationTypeSetCorrectly(
            CatalogueItem catalogueItem)
        {
            var actual = new MobileTabletBasedModel(catalogueItem);

            actual.ApplicationType.Should().Be(ApplicationType.MobileTablet);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new MobileTabletBasedModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
