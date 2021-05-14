using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class ConnectivityModel : MarketingBaseModel
    {
        public ConnectivityModel() : base(null)
        {
        }

        public ConnectivityModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            ConnectionSpeeds = new List<SelectListItem>
            {
                new() { Text = "Please select"},
                new() { Text = "0.5Mbps", Value="0.5Mbps"},
                new() { Text = "1Mbps", Value="1Mbps"},
                new() { Text = "1.5Mbps", Value="1.5Mbps"},
                new() { Text = "2Mbps", Value="2Mbps"},
                new() { Text = "3Mbps", Value="3Mbps"},
                new() { Text = "5Mbps", Value="5Mbps"},
                new() { Text = "8Mbps", Value="8Mbps"},
                new() { Text = "10Mbps", Value="10Mbps"},
                new() { Text = "15Mbps", Value="15Mbps"},
                new() { Text = "20Mbps", Value="20Mbps"},
                new() { Text = "30Mbps", Value="30Mbps"},
                new() { Text = "Higher than 30Mbps", Value="Higher than 30Mbps"}
            };

            SelectedConnectionSpeed = ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed;

            ConnectionTypes = new ConnectionTypeModel[]
            {
                new() { ConnectionType = "GPRS" },
                new() { ConnectionType = "3G" },
                new() { ConnectionType = "LTE" },
                new() { ConnectionType = "4G" },
                new() { ConnectionType = "5G" },
                new() { ConnectionType = "Bluetooth" },
                new() { ConnectionType = "Wifi" }
            };

            CheckConnectionTypes();

            Description = ClientApplication?.MobileConnectionDetails?.Description;            
        }

        public override bool? IsComplete
        {
            get 
            {
                if (!string.IsNullOrWhiteSpace(ClientApplication?.MobileConnectionDetails?.MinimumConnectionSpeed) ||
                  !string.IsNullOrWhiteSpace(ClientApplication?.MobileConnectionDetails?.Description))
                    return true;

                return ClientApplication?.MobileConnectionDetails?.ConnectionType?.Any();
            }
        }

        public string SelectedConnectionSpeed { get; set; }
        public List<SelectListItem> ConnectionSpeeds { get; set; }

        public ConnectionTypeModel[] ConnectionTypes { get; set; }

        public string Description { get; set; }

        private void CheckConnectionTypes()
        {
            foreach (var connectionType in ConnectionTypes)
            {
                if (ClientApplication?.MobileConnectionDetails?.ConnectionType != null &&
                    ClientApplication.MobileConnectionDetails.ConnectionType.Any(x => x.Equals(connectionType.ConnectionType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    connectionType.Checked = true;
                }
            }
        }
    }
}
