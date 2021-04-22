using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class ThirdPartyModel : MarketingBaseModel
    {
        public ThirdPartyModel() : base(null)
        {
        }

        public ThirdPartyModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(ClientApplication.MobileThirdParty?.ThirdPartyComponents) ||
                  !string.IsNullOrWhiteSpace(ClientApplication.MobileThirdParty?.DeviceCapabilities);
            }
        }        
    }
}
