using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class ConnectivityModel : MarketingBaseModel
    {
        public ConnectivityModel()
            : base(null)
        {
        }

        public ConnectivityModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            SelectedConnectionSpeed = ClientApplication?.NativeDesktopMinimumConnectionSpeed;
        }

        public List<SelectListItem> ConnectionSpeeds { get; set; }

        public string SelectedConnectionSpeed { get; set; }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMinimumConnectionSpeed);
    }
}
