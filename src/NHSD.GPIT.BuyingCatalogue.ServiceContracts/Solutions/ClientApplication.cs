using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class ClientApplication
    {
        public HashSet<string> ClientApplicationTypes { get; set; } = new();

        public HashSet<string> BrowsersSupported { get; set; } = new();

        public bool? MobileResponsive { get; set; }

        public virtual Plugins Plugins { get; set; } 

        public virtual string HardwareRequirements { get; set; }

        public string NativeMobileHardwareRequirements { get; set; }

        public string NativeDesktopHardwareRequirements { get; set; }

        public virtual string AdditionalInformation { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public string MinimumDesktopResolution { get; set; }

        public virtual bool? MobileFirstDesign { get; set; }

        public bool? NativeMobileFirstDesign { get; set; }

        public MobileOperatingSystems MobileOperatingSystems { get; set; }

        public MobileConnectionDetails MobileConnectionDetails { get; set; }

        public MobileMemoryAndStorage MobileMemoryAndStorage { get; set; } 

        public MobileThirdParty MobileThirdParty { get; set; } = new();

        public string NativeMobileAdditionalInformation { get; set; }

        public string NativeDesktopOperatingSystemsDescription { get; set; }

        public string NativeDesktopMinimumConnectionSpeed { get; set; }

        public NativeDesktopThirdParty NativeDesktopThirdParty { get; set; } 

        public NativeDesktopMemoryAndStorage NativeDesktopMemoryAndStorage { get; set; } 

        public string NativeDesktopAdditionalInformation { get; set; }

        public virtual bool AdditionalInformationComplete() => !string.IsNullOrWhiteSpace(AdditionalInformation);
        
        public virtual bool? BrowserBasedModelComplete() =>
            SupportedBrowsersComplete() &&
            MobileFirstDesignComplete() &&
            PlugInsComplete().GetValueOrDefault() &&
            ConnectivityAndResolutionComplete();

        public virtual bool ConnectivityAndResolutionComplete() => !string.IsNullOrWhiteSpace(MinimumConnectionSpeed);

        public virtual bool? HardwareRequirementsComplete() => !string.IsNullOrWhiteSpace(HardwareRequirements);

        public virtual bool? NativeDesktopAdditionalInformationComplete() =>
            !string.IsNullOrWhiteSpace(NativeDesktopAdditionalInformation);

        public virtual bool? NativeDesktopConnectivityComplete() =>
            !string.IsNullOrWhiteSpace(NativeDesktopMinimumConnectionSpeed);

        public virtual bool? NativeDesktopHardwareRequirementsComplete() =>
            !string.IsNullOrWhiteSpace(NativeDesktopHardwareRequirements);

        public virtual bool? NativeDesktopMemoryAndStorageComplete() => NativeDesktopMemoryAndStorage?.IsValid();
        
        public virtual bool? NativeMobileMemoryAndStorageComplete() => MobileMemoryAndStorage?.IsValid();

        public virtual bool? NativeDesktopSupportedOperatingSystemsComplete() =>
            !string.IsNullOrWhiteSpace(NativeDesktopOperatingSystemsDescription);
        
        public virtual bool? NativeDesktopThirdPartyComplete() => NativeDesktopThirdParty?.IsValid();

        public virtual bool? NativeMobileConnectivityComplete() => MobileConnectionDetails?.IsValid();

        public virtual bool NativeMobileFirstApproachComplete() => NativeMobileFirstDesign.HasValue;

        public virtual bool MobileFirstDesignComplete() => MobileFirstDesign.HasValue;

        public virtual bool? NativeMobileSupportedOperatingSystemsComplete() =>
            MobileOperatingSystems?.OperatingSystems?.Any();

        public virtual bool NativeMobileAdditionalInformationComplete() =>
            !string.IsNullOrWhiteSpace(NativeMobileAdditionalInformation);

        public virtual bool? NativeMobileHardwareRequirementsComplete() =>
            !string.IsNullOrWhiteSpace(NativeMobileHardwareRequirements);
        
        public virtual bool? PlugInsComplete() => Plugins?.Required.HasValue;

        public virtual bool SupportedBrowsersComplete() =>
            BrowsersSupported != null && BrowsersSupported.Any() &&
            MobileResponsive.HasValue;
        
        public virtual bool? NativeMobileThirdPartyComplete() => MobileThirdParty?.IsValid();
    }
}
