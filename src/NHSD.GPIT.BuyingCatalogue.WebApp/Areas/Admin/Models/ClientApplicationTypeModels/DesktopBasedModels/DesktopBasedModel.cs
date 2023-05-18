using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels
{
    public sealed class DesktopBasedModel : ApplicationTypeBaseModel
    {
        public DesktopBasedModel()
        {
        }

        public DesktopBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            ApplicationType = ClientApplicationType.Desktop;
        }

        public TaskProgress SupportedOperatingSystemsStatus() => ClientApplicationProgress.NativeDesktopSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplicationProgress.NativeDesktopConnectivityStatus();

        public TaskProgress MemoryAndStorageStatus() => ClientApplicationProgress.NativeDesktopMemoryAndStorageStatus();

        public TaskProgress StatusThirdParty() => ClientApplicationProgress.NativeDesktopThirdPartyStatus();

        public TaskProgress StatusHardware() => ClientApplicationProgress.NativeDesktopHardwareRequirementsStatus();

        public TaskProgress StatusAdditionalInformation() => ClientApplicationProgress.NativeDesktopAdditionalInformationStatus();
    }
}
