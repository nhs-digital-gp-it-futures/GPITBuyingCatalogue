using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class ClientApplication
    {
        public HashSet<string> ClientApplicationTypes { get; set; } = new();

        public HashSet<string> BrowsersSupported { get; set; } = new();

        public bool? MobileResponsive { get; set; }

        public Plugins Plugins { get; set; }

        public string HardwareRequirements { get; set; }

        public string NativeMobileHardwareRequirements { get; set; }

        public string NativeDesktopHardwareRequirements { get; set; }

        public string AdditionalInformation { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public string MinimumDesktopResolution { get; set; }

        public bool? MobileFirstDesign { get; set; }

        public bool? NativeMobileFirstDesign { get; set; }

        public MobileOperatingSystems MobileOperatingSystems { get; set; }

        public MobileConnectionDetails MobileConnectionDetails { get; set; }

        public MobileMemoryAndStorage MobileMemoryAndStorage { get; set; }

        public MobileThirdParty MobileThirdParty { get; set; }

        public string NativeMobileAdditionalInformation { get; set; }

        public string NativeDesktopOperatingSystemsDescription { get; set; }

        public string NativeDesktopMinimumConnectionSpeed { get; set; }

        public NativeDesktopThirdParty NativeDesktopThirdParty { get; set; }

        public NativeDesktopMemoryAndStorage NativeDesktopMemoryAndStorage { get; set; }

        public string NativeDesktopAdditionalInformation { get; set; }
    }
}
