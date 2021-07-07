using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class ConnectivityAndResolutionModel : MarketingBaseModel
    {
        public ConnectivityAndResolutionModel()
            : base(null)
        {
        }

        public ConnectivityAndResolutionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            SelectedConnectionSpeed = ClientApplication.MinimumConnectionSpeed;

            SelectedScreenResolution = ClientApplication.MinimumDesktopResolution;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(ClientApplication.MinimumConnectionSpeed);

        public string SelectedConnectionSpeed { get; set; }

        public List<SelectListItem> ConnectionSpeeds { get; set; }

        public string SelectedScreenResolution { get; set; }

        public List<SelectListItem> ScreenResolutions { get; set; }
    }
}
