using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public sealed class NativeMobileModel : MarketingBaseModel
    {
        public NativeMobileModel()
            : base(null)
        {
        }

        public override bool? IsComplete =>
            ClientApplication != null &&
            ClientApplication.NativeMobileSupportedOperatingSystemsComplete() &&
            ClientApplication.NativeMobileFirstApproachComplete() &&
            ClientApplication.NativeMobileMemoryAndStorageComplete().GetValueOrDefault();

        public string SupportedOperatingSystemsStatus =>
            (ClientApplication?.NativeMobileSupportedOperatingSystemsComplete()).ToStatus();

        public string MobileFirstApproachStatus => (ClientApplication?.NativeMobileFirstApproachComplete()).ToStatus();

        public string ConnectivityStatus => (ClientApplication?.NativeMobileConnectivityComplete()).ToStatus();

        public string MemoryAndStorageStatus => (ClientApplication?.NativeMobileMemoryAndStorageComplete()).ToStatus();

        public string ThirdPartyStatus => (ClientApplication?.NativeMobileThirdPartyComplete()).ToStatus();

        public string HardwareRequirementsStatus =>
            (ClientApplication?.NativeMobileHardwareRequirementsComplete()).ToStatus();

        public string AdditionalInformationStatus =>
            (ClientApplication?.NativeMobileAdditionalInformationComplete()).ToStatus();
    }
}
