using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class NativeDesktopModel : MarketingBaseModel
    {
        public NativeDesktopModel() : base(null)
        {
        }
        
        public NativeDesktopModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                                
        }

        public override bool? IsComplete =>
            new OperatingSystemsModel(CatalogueItem).IsComplete.GetValueOrDefault() &&
            new ConnectivityModel(CatalogueItem).IsComplete.GetValueOrDefault() &&                    
            new MemoryAndStorageModel(CatalogueItem).IsComplete.GetValueOrDefault();

        public string SupportedOperatingSystemsStatus => GetStatus(new OperatingSystemsModel(CatalogueItem));

        public string ConnectivityStatus => GetStatus(new ConnectivityModel(CatalogueItem));

        public string MemoryStatus => GetStatus(new MemoryAndStorageModel(CatalogueItem));

        public string ThirdPartyStatus => GetStatus(new ThirdPartyModel(CatalogueItem));

        public string HardwareRequirementsStatus => GetStatus(new HardwareRequirementsModel(CatalogueItem));

        public string AdditionalInformationStatus => GetStatus(new AdditionalInformationModel(CatalogueItem));
    }
}
