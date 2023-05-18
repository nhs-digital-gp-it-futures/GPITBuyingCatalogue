using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.MobileTabletBasedModels
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

            ApplicationType = ClientApplicationType.MobileTablet;
        }

        public TaskProgress SupportedOperatingSystemsStatus() => ClientApplicationProgress.NativeMobileSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplicationProgress.NativeMobileConnectivityStatus();

        public TaskProgress MemoryStatus() => ClientApplicationProgress.NativeMobileMemoryAndStorageStatus();

        public TaskProgress ThirdPartyStatus() => ClientApplicationProgress.NativeMobileThirdPartyStatus();

        public TaskProgress HardwareStatus() => ClientApplicationProgress.NativeMobileHardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ClientApplicationProgress.NativeMobileAdditionalInformationStatus();
    }
}
