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

        public TaskProgress SupportedOperatingSystemsStatus() => ClientApplication.NativeMobileSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplication.NativeMobileConnectivityStatus();

        public TaskProgress MemoryStatus() => ClientApplication.NativeMobileMemoryAndStorageStatus();

        public TaskProgress ThirdPartyStatus() => ClientApplication.NativeMobileThirdPartyStatus();

        public TaskProgress HardwareStatus() => ClientApplication.NativeMobileHardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ClientApplication.NativeMobileAdditionalInformationStatus();
    }
}
