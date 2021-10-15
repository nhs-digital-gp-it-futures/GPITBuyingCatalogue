using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.BrowserBasedModels
{
    public static class ConnectivityAndResolutionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = new ConnectivityAndResolutionModel(catalogueItem);

            actual.SelectedConnectionSpeed.Should().Be(catalogueItem.Solution.GetClientApplication().MinimumConnectionSpeed);
            actual.SelectedScreenResolution.Should().Be(catalogueItem.Solution.GetClientApplication().MinimumDesktopResolution);
            actual.ConnectionSpeeds.Should().BeEquivalentTo(SelectLists.ConnectionSpeeds);
            actual.ScreenResolutions.Should().BeEquivalentTo(SelectLists.ScreenResolutions);
            actual.BackLink.Should().Be("./");
            actual.BackLinkText.Should().Be("Go back");
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ConnectivityAndResolutionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
