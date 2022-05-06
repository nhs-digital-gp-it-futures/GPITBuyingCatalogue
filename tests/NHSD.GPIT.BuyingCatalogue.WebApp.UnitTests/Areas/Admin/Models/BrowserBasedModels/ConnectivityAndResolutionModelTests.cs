﻿using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.BrowserBasedModels
{
    public static class ConnectivityAndResolutionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var actual = new ConnectivityAndResolutionModel(solution.CatalogueItem);

            actual.SelectedConnectionSpeed.Should().Be(solution.GetClientApplication().MinimumConnectionSpeed);
            actual.SelectedScreenResolution.Should().Be(solution.GetClientApplication().MinimumDesktopResolution);
            actual.ConnectionSpeeds.Should().BeEquivalentTo(SelectLists.ConnectionSpeeds);
            actual.ScreenResolutions.Should().BeEquivalentTo(SelectLists.ScreenResolutions);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ConnectivityAndResolutionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
