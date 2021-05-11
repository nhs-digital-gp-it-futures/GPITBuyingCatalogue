using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class MobileThirdParty
    {
        public string ThirdPartyComponents { get; set; }

        public string DeviceCapabilities { get; set; }

        public virtual bool IsValid() =>
            !string.IsNullOrWhiteSpace(ThirdPartyComponents) ||
            !string.IsNullOrWhiteSpace(DeviceCapabilities);
    }
}
