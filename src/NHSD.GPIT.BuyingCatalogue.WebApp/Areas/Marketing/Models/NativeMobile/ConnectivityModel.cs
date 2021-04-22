using System;
using System.Linq;
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
    }
}
