using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class ThirdPartyModel : MarketingBaseModel
    {
        public ThirdPartyModel() : base(null)
        {
        }

        public ThirdPartyModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";                                    
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopThirdParty?.ThirdPartyComponents) ||
                  !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopThirdParty?.DeviceCapabilities);
            }
        }        
    }
}
