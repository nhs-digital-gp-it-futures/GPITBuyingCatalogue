using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
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

            var mobileConnectionDetails = catalogueItem.Solution.GetClientApplication().MobileConnectionDetails;

            actual.SelectedConnectionSpeed.Should().Be(mobileConnectionDetails.MinimumConnectionSpeed);
            actual.Description.Should().Be(mobileConnectionDetails.Description);

            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet");

            actual.ConnectionTypes.Should().BeEquivalentTo(new ConnectionTypeModel[]
            {
                new() { ConnectionType = "GPRS", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("GPRS")) },
                new() { ConnectionType = "3G", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("3G")) },
                new() { ConnectionType = "LTE", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("LTE")) },
                new() { ConnectionType = "4G", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("4G")) },
                new() { ConnectionType = "5G", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("5G")) },
                new() { ConnectionType = "Bluetooth", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("Bluetooth")) },
                new() { ConnectionType = "Wifi", Checked = mobileConnectionDetails.ConnectionType.Any(t => t.EqualsIgnoreCase("WiFi")) },
            });
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

        [Theory]
        [CommonAutoData]
        public static void IsComplete_CorrectlySet_WhenAConnectionTypeChecked(
            ConnectivityModel model)
        {
            model.SelectedConnectionSpeed = null;
            model.ConnectionTypes.First().Checked = true;

            var actual = model.IsComplete;

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsComplete_CorrectlySet_WhenNoConnectionTypeChecked(
          ConnectivityModel model)
        {
            model.SelectedConnectionSpeed = null;
            Array.ForEach(model.ConnectionTypes, c => c.Checked = false);

            var actual = model.IsComplete;

            actual.Should().BeFalse();
        }
    }
}
