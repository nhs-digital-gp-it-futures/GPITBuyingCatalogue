using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DeleteApplicationTypeModels
{
    public static class DeleteApplicationTypeConfirmationModelTests
    {
        [Theory]
        [MockAutoData]
        public static void BrowserBased_PropertiesSetCorrectly(
            CatalogueItem catalogueItem)
        {
            var actual = new DeleteApplicationTypeConfirmationModel(catalogueItem, ApplicationType.BrowserBased);
            actual.ApplicationType.Should().Be("browser-based");
        }

        [Theory]
        [MockAutoData]
        public static void Desktop_PropertiesSetCorrectly(
            CatalogueItem catalogueItem)
        {
            var actual = new DeleteApplicationTypeConfirmationModel(catalogueItem, ApplicationType.Desktop);
            actual.ApplicationType.Should().Be("desktop");
        }

        [Theory]
        [MockAutoData]
        public static void Mobile_PropertiesSetCorrectly(
            CatalogueItem catalogueItem)
        {
            var actual = new DeleteApplicationTypeConfirmationModel(catalogueItem, ApplicationType.MobileTablet);
            actual.ApplicationType.Should().Be("mobile or tablet");
        }
    }
}
