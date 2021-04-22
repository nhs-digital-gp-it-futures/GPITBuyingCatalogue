using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class SupportedBrowsersModel : MarketingBaseModel
    {
        public SupportedBrowsersModel() : base(null)
        {
        }

        public SupportedBrowsersModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";
        }

        public override bool? IsComplete
        {
            get { return ClientApplication.BrowsersSupported?.Any(); }
        }
    }
}
