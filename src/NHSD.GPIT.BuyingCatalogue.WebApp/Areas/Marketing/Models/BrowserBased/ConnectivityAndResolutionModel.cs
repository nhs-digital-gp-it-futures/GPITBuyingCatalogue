using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class ConnectivityAndResolutionModel : MarketingBaseModel
    {
        public ConnectivityAndResolutionModel() : base(null)
        {
        }

        public ConnectivityAndResolutionModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";                                 
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }        
    }
}
