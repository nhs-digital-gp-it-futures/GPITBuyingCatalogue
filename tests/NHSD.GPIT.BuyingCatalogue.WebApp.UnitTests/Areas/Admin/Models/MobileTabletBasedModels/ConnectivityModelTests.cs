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
        [InlineData("2Mbps", false, true)]
        [InlineData("", false, false)]
        [InlineData(" ", false, false)]
        [InlineData(null, false, false)]
        [InlineData(null, true, true)]
        [InlineData("2Mbs", true, true)]
        public static void IsComplete_CorrectlySet(
            string selectedConnectionSpeed,
            bool hasConnectionTypes,
            bool expectedCompletionState)
        {
            var connectionTypes = hasConnectionTypes ? new ConnectionTypeModel[1] { new ConnectionTypeModel() } : Array.Empty<ConnectionTypeModel>();

            var model = new ConnectivityModel { SelectedConnectionSpeed = selectedConnectionSpeed, ConnectionTypes = connectionTypes };

            var actual = model.IsComplete;

            actual.Should().Be(expectedCompletionState);
        }
    }
}
