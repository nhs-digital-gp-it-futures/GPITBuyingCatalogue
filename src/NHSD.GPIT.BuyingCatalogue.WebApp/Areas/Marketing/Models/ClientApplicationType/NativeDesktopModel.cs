using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class NativeDesktopModel : MarketingBaseModel
    {
        public NativeDesktopModel()
            : base(null)
        {
        }

        public NativeDesktopModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
        }

        public override bool? IsComplete =>
            ClientApplication != null &&
            ClientApplication.NativeDesktopSupportedOperatingSystemsComplete().GetValueOrDefault() &&
            ClientApplication.NativeDesktopConnectivityComplete().GetValueOrDefault() &&
            ClientApplication.NativeDesktopMemoryAndStorageComplete().GetValueOrDefault();

        public string SupportedOperatingSystemsStatus =>
            (ClientApplication?.NativeDesktopSupportedOperatingSystemsComplete()).ToStatus();

        public string ConnectivityStatus => (ClientApplication?.NativeDesktopConnectivityComplete()).ToStatus();

        public string MemoryAndStorageStatus => (ClientApplication?.NativeDesktopMemoryAndStorageComplete()).ToStatus();

        public string ThirdPartyStatus => (ClientApplication?.NativeDesktopThirdPartyComplete()).ToStatus();

        public string HardwareRequirementsStatus =>
            (ClientApplication?.NativeDesktopHardwareRequirementsComplete()).ToStatus();

        public string AdditionalInformationStatus =>
            (ClientApplication?.NativeDesktopAdditionalInformationComplete()).ToStatus();
    }
}
