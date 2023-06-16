using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels
{
    public sealed class MobileTabletBasedModel : ApplicationTypeBaseModel
    {
        public MobileTabletBasedModel()
        {
        }

        public MobileTabletBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            ApplicationType = ApplicationType.MobileTablet;
        }

        public TaskProgress SupportedOperatingSystemsStatus() => ApplicationTypes.NativeMobileSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ApplicationTypes.NativeMobileConnectivityStatus();

        public TaskProgress MemoryStatus() => ApplicationTypes.NativeMobileMemoryAndStorageStatus();

        public TaskProgress ThirdPartyStatus() => ApplicationTypes.NativeMobileThirdPartyStatus();

        public TaskProgress HardwareStatus() => ApplicationTypes.NativeMobileHardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ApplicationTypes.NativeMobileAdditionalInformationStatus();
    }
}
