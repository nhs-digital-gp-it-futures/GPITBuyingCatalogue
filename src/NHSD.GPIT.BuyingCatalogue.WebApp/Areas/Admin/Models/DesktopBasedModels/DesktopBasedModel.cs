using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

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
        }

        public override bool IsComplete =>
            (ClientApplication?.NativeDesktopSupportedOperatingSystemsComplete() ?? false) &&
            (ClientApplication?.NativeDesktopMemoryAndStorageComplete() ?? false) &&
            (ClientApplication?.NativeDesktopConnectivityComplete() ?? false);

        public string SupportedOperatingSystemsStatus => (ClientApplication?.NativeDesktopSupportedOperatingSystemsComplete()).ToStatus();

        public string MemoryAndStorageStatus => (ClientApplication?.NativeDesktopMemoryAndStorageComplete()).ToStatus();

        public string ConnectivityStatus => (ClientApplication?.NativeDesktopConnectivityComplete()).ToStatus();

        public string ThirdPartyStatus => (ClientApplication?.NativeDesktopThirdPartyComplete()).ToStatus();

        public string HardwareRequirementsStatus => (ClientApplication?.NativeDesktopHardwareRequirementsComplete()).ToStatus();

        public string AdditionalInformationStatus => (ClientApplication?.NativeDesktopAdditionalInformationComplete()).ToStatus();
    }
}
