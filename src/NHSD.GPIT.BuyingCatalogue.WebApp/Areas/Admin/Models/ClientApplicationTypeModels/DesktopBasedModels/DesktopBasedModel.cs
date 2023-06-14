using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels
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

            ApplicationType = ApplicationType.Desktop;
        }

        public TaskProgress SupportedOperatingSystemsStatus() => ClientApplication.NativeDesktopSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplication.NativeDesktopConnectivityStatus();

        public TaskProgress MemoryAndStorageStatus() => ClientApplication.NativeDesktopMemoryAndStorageStatus();

        public TaskProgress StatusThirdParty() => ClientApplication.NativeDesktopThirdPartyStatus();

        public TaskProgress StatusHardware() => ClientApplication.NativeDesktopHardwareRequirementsStatus();

        public TaskProgress StatusAdditionalInformation() => ClientApplication.NativeDesktopAdditionalInformationStatus();
    }
}
