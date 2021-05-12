using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
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

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            ConnectionSpeeds = new List<SelectListItem>
            {
                new () { Text = "Please select"},
                new () { Text = "0.5Mbps", Value="0.5Mbps"},
                new () { Text = "1Mbps", Value="1Mbps"},
                new () { Text = "1.5Mbps", Value="1.5Mbps"},
                new () { Text = "2Mbps", Value="2Mbps"},
                new () { Text = "3Mbps", Value="3Mbps"},
                new () { Text = "5Mbps", Value="5Mbps"},
                new () { Text = "8Mbps", Value="8Mbps"},
                new () { Text = "10Mbps", Value="10Mbps"},
                new () { Text = "15Mbps", Value="15Mbps"},
                new () { Text = "20Mbps", Value="20Mbps"},
                new () { Text = "30Mbps", Value="30Mbps"},
                new () { Text = "Higher than 30Mbps", Value="Higher than 30Mbps"}
            };

            SelectedConnectionSpeed = ClientApplication?.NativeDesktopMinimumConnectionSpeed;
        }

        public List<SelectListItem> ConnectionSpeeds { get; set; }

        public string SelectedConnectionSpeed { get; set; }
        
        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMinimumConnectionSpeed);
    }
}
