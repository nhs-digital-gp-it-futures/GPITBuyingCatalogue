using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels
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

            var clientApplicationTypes = catalogueItem.Solution?.GetClientApplication()?.ClientApplicationTypes;

            if (clientApplicationTypes?.Any(type => type.Equals("native-desktop", StringComparison.OrdinalIgnoreCase)) ?? false)
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type";
            else
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/add-application-type";

            ApplicationType = ClientApplicationType.Desktop;
        }

        public TaskProgress SupportedOperatingSystemsStatus() => ClientApplication.NativeDesktopSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplication.NativeDesktopConnectivityStatus();

        public TaskProgress MemoryAndStorageStatus() => ClientApplication.NativeDesktopMemoryAndStorageStatus();

        public TaskProgress StatusThirdParty() => ClientApplication.NativeDesktopThirdPartyStatus();

        public TaskProgress StatusHardware() => ClientApplication.NativeDesktopHardwareRequirementsStatus();

        public TaskProgress StatusAdditionalInformation() => ClientApplication.NativeDesktopAdditionalInformationStatus();
    }
}
