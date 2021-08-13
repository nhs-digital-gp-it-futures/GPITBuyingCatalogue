using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class ClientApplication
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

        // TODO: refactor to avoid what is essentially duplication of ClientApplicationTypes
        // (consider ClientApplicationTypes and use of ClientApplicationType enum)
        public IReadOnlyList<ClientApplicationType> ExistingClientApplicationTypes
        {
            get
            {
                var result = new List<ClientApplicationType>(3);

                if (ClientApplicationTypes?.Any(type => type.Equals("browser-based", StringComparison.OrdinalIgnoreCase)) ?? false)
                    result.Add(ClientApplicationType.BrowserBased);

                if (ClientApplicationTypes?.Any(type => type.Equals("native-mobile", StringComparison.OrdinalIgnoreCase)) ?? false)
                    result.Add(ClientApplicationType.MobileTablet);

                if (ClientApplicationTypes?.Any(type => type.Equals("native-desktop", StringComparison.OrdinalIgnoreCase)) ?? false)
                    result.Add(ClientApplicationType.Desktop);

                return result;
            }
        }

        public void EnsureClientApplicationTypePresent(ClientApplicationType clientApplicationType)
        {
            if (ClientApplicationTypes is null)
                ClientApplicationTypes = new HashSet<string>();

            if (!ClientApplicationTypes.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)))
                ClientApplicationTypes.Add(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        public bool AdditionalInformationComplete() => !string.IsNullOrWhiteSpace(AdditionalInformation);

        public bool? BrowserBasedModelComplete() =>
            SupportedBrowsersComplete() &&
            MobileFirstDesignComplete() &&
            PlugInsComplete().GetValueOrDefault() &&
            ConnectivityAndResolutionComplete();

        public bool ConnectivityAndResolutionComplete() => !string.IsNullOrWhiteSpace(MinimumConnectionSpeed);

        public bool? HardwareRequirementsComplete() => !string.IsNullOrWhiteSpace(HardwareRequirements);

        public bool NativeDesktopAdditionalInformationComplete() => !string.IsNullOrWhiteSpace(NativeDesktopAdditionalInformation);

        public bool? NativeDesktopConnectivityComplete() => !string.IsNullOrWhiteSpace(NativeDesktopMinimumConnectionSpeed);

        public bool? NativeDesktopHardwareRequirementsComplete() => !string.IsNullOrWhiteSpace(NativeDesktopHardwareRequirements);

        public bool? NativeDesktopMemoryAndStorageComplete() => NativeDesktopMemoryAndStorage?.IsValid();

        public bool? NativeMobileMemoryAndStorageComplete() => MobileMemoryAndStorage?.IsValid();

        public bool? NativeDesktopSupportedOperatingSystemsComplete() => !string.IsNullOrWhiteSpace(NativeDesktopOperatingSystemsDescription);

        public bool? NativeDesktopThirdPartyComplete() => NativeDesktopThirdParty?.IsValid();

        public bool? NativeMobileConnectivityComplete() => MobileConnectionDetails?.IsValid();

        public bool NativeMobileFirstApproachComplete() => NativeMobileFirstDesign.HasValue;

        public bool MobileFirstDesignComplete() => MobileFirstDesign.HasValue;

        public bool NativeMobileSupportedOperatingSystemsComplete() => MobileOperatingSystems?.OperatingSystems?.Any() ?? false;

        public bool NativeMobileAdditionalInformationComplete() => !string.IsNullOrWhiteSpace(NativeMobileAdditionalInformation);

        public bool? NativeMobileHardwareRequirementsComplete() => !string.IsNullOrWhiteSpace(NativeMobileHardwareRequirements);

        public bool? PlugInsComplete() => Plugins?.Required.HasValue;

        public bool SupportedBrowsersComplete() =>
            BrowsersSupported != null && BrowsersSupported.Any() &&
            MobileResponsive.HasValue;

        public bool? NativeMobileThirdPartyComplete() => MobileThirdParty?.IsValid();
    }
}
