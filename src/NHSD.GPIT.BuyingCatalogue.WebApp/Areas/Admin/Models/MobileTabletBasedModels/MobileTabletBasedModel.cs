using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
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

            var clientApplicationTypes = catalogueItem.Solution?.GetClientApplication()?.ClientApplicationTypes;

            if (clientApplicationTypes?.Any(type => type.Equals("native-mobile", StringComparison.OrdinalIgnoreCase)) ?? false)
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type";
            else
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/add-application-type";

            ApplicationType = ClientApplicationType.MobileTablet;
        }

        public TaskProgress SupportedOperatingSystemsStatus() => ClientApplication.NativeMobileSupportedOperatingSystemsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplication.NativeMobileConnectivityStatus();

        public TaskProgress MemoryStatus() => ClientApplication.NativeMobileMemoryAndStorageStatus();

        public TaskProgress ThirdPartyStatus() => ClientApplication.NativeMobileThirdPartyStatus();

        public TaskProgress HardwareStatus() => ClientApplication.NativeMobileHardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ClientApplication.NativeMobileAdditionalInformationStatus();
    }
}
