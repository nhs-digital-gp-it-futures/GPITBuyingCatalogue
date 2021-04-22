using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class MemoryAndStorageModel : MarketingBaseModel
    {
        public MemoryAndStorageModel() : base(null)
        {
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(ClientApplication.MobileMemoryAndStorage?.MinimumMemoryRequirement) &&
                  !string.IsNullOrWhiteSpace(ClientApplication.MobileMemoryAndStorage?.Description);
            }
        }        
    }
}
