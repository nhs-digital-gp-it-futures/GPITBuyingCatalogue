using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DesktopBasedModels
{
    public static class ConnectivityModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = new ConnectivityModel(catalogueItem);

            actual.ConnectionSpeeds.Should().BeEquivalentTo(Framework.Constants.SelectLists.ConnectionSpeeds);
            actual.SelectedConnectionSpeed.Should().Be(catalogueItem.Solution.GetClientApplication().NativeDesktopMinimumConnectionSpeed);
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop");
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ConnectivityModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [InlineData("2Mbps", true)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        public static void IsComplete_CorrectlySet_WhenConnectionSpeedSet(
            string selectedConnectionSpeed,
            bool expectedCompletionState)
        {
            var model = new ConnectivityModel { SelectedConnectionSpeed = selectedConnectionSpeed };

            var actual = model.IsComplete;

            actual.Should().Be(expectedCompletionState);
        }
    }
}
