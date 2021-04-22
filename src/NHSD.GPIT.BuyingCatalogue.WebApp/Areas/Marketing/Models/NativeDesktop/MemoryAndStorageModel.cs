using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class MemoryAndStorageModel : MarketingBaseModel
    {
        public MemoryAndStorageModel() : base(null)
        {
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement)
                  && !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription)
                  && !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu); 
            }
        }        
    }
}
