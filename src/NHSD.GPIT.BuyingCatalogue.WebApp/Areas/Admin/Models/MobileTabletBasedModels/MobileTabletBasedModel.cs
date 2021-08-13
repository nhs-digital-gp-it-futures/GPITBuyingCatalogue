using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public class MobileTabletBasedModel : ApplicationTypeBaseModel
    {
        public MobileTabletBasedModel()
            : base(null)
        {
        }

        public MobileTabletBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            var clientApplicationTypes = catalogueItem.Solution?.GetClientApplication()?.ClientApplicationTypes;

            if (clientApplicationTypes?.Any(type => type.Equals("native-mobile", StringComparison.OrdinalIgnoreCase)) ?? false)
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type";
            else
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/add-application-type";
        }

        public override bool IsComplete =>
            (ClientApplication?.NativeMobileSupportedOperatingSystemsComplete() ?? false) &&
            (ClientApplication?.NativeMobileMemoryAndStorageComplete() ?? false);

        public string SupportedOperatingSystemsStatus => (ClientApplication?.NativeMobileSupportedOperatingSystemsComplete()).ToStatus();

        public string MemoryAndStorageStatus => (ClientApplication?.NativeMobileMemoryAndStorageComplete()).ToStatus();

        public string ConnectivityStatus => (ClientApplication?.NativeMobileConnectivityComplete()).ToStatus();

        public string ThirdPartyStatus => (ClientApplication?.NativeMobileThirdPartyComplete()).ToStatus();

        public string HardwareRequirementsStatus => (ClientApplication?.NativeMobileHardwareRequirementsComplete()).ToStatus();

        public string AdditionalInformationStatus => (ClientApplication?.NativeMobileAdditionalInformationComplete()).ToStatus();
    }
}
