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

        public TaskProgress SupportedOperatingSystemsStatus() => ApplicationTypes.NativeDesktopSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ApplicationTypes.NativeDesktopConnectivityStatus();

        public TaskProgress MemoryAndStorageStatus() => ApplicationTypes.NativeDesktopMemoryAndStorageStatus();

        public TaskProgress StatusThirdParty() => ApplicationTypes.NativeDesktopThirdPartyStatus();

        public TaskProgress StatusHardware() => ApplicationTypes.NativeDesktopHardwareRequirementsStatus();

        public TaskProgress StatusAdditionalInformation() => ApplicationTypes.NativeDesktopAdditionalInformationStatus();
    }
}
