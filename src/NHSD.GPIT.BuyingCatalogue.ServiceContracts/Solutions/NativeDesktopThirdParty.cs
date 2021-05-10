using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class NativeDesktopThirdParty
    {
        public string DeviceCapabilities { get; set; }
        
        public string ThirdPartyComponents { get; set; }

        public virtual bool IsValid() =>
            !string.IsNullOrWhiteSpace(DeviceCapabilities) ||
            !string.IsNullOrWhiteSpace(ThirdPartyComponents);
    }
}
