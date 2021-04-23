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
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            ConnectionSpeeds = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Please select"},
                new SelectListItem{ Text = "0.5Mbps", Value="0.5Mbps"},
                new SelectListItem{ Text = "1Mbps", Value="1Mbps"},
                new SelectListItem{ Text = "1.5Mbps", Value="1.5Mbps"},
                new SelectListItem{ Text = "2Mbps", Value="2Mbps"},
                new SelectListItem{ Text = "3Mbps", Value="3Mbps"},
                new SelectListItem{ Text = "5Mbps", Value="5Mbps"},
                new SelectListItem{ Text = "8Mbps", Value="8Mbps"},
                new SelectListItem{ Text = "10Mbps", Value="10Mbps"},
                new SelectListItem{ Text = "15Mbps", Value="15Mbps"},
                new SelectListItem{ Text = "20Mbps", Value="20Mbps"},
                new SelectListItem{ Text = "30Mbps", Value="30Mbps"},
                new SelectListItem{ Text = "Higher than 30Mbps", Value="Higher than 30Mbps"}
            };

            SelectedConnectionSpeed = ClientApplication.MobileConnectionDetails?.MinimumConnectionSpeed;


            ConnectionTypes = new ConnectionTypeModel[]
            {
                new ConnectionTypeModel{ ConnectionType = "GPRS" },
                new ConnectionTypeModel{ ConnectionType = "3G" },
                new ConnectionTypeModel{ ConnectionType = "LTE" },
                new ConnectionTypeModel{ ConnectionType = "4G" },
                new ConnectionTypeModel{ ConnectionType = "5G" },
                new ConnectionTypeModel{ ConnectionType = "Bluetooth" },
                new ConnectionTypeModel{ ConnectionType = "Wifi" }
            };

            CheckConnectionTypes();

            Description = ClientApplication.MobileConnectionDetails?.Description;            
        }

        public override bool? IsComplete
        {
            get 
            {
                if (!string.IsNullOrWhiteSpace(ClientApplication.MobileConnectionDetails?.MinimumConnectionSpeed) ||
                  !string.IsNullOrWhiteSpace(ClientApplication.MobileConnectionDetails?.Description))
                    return true;

                return ClientApplication.MobileConnectionDetails?.ConnectionType?.Any();
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
                if (ClientApplication.MobileConnectionDetails?.ConnectionType != null &&
                    ClientApplication.MobileConnectionDetails.ConnectionType.Any(x => x.Equals(connectionType.ConnectionType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    connectionType.Checked = true;
                }
            }
        }
    }
}
