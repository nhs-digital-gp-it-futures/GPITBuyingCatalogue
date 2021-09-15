using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

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
            ClientApplicationTypes ??= new HashSet<string>();

            if (!ClientApplicationTypes.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)))
                ClientApplicationTypes.Add(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        public TaskProgress AdditionalInformationStatus() =>
            !string.IsNullOrWhiteSpace(AdditionalInformation) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress ConnectivityStatus() =>
            !string.IsNullOrWhiteSpace(MinimumConnectionSpeed) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress HardwareRequirementsStatus() =>
            !string.IsNullOrWhiteSpace(HardwareRequirements) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeDesktopAdditionalInformationStatus() =>
            !string.IsNullOrWhiteSpace(NativeDesktopAdditionalInformation) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeDesktopConnectivityStatus() =>
            !string.IsNullOrWhiteSpace(NativeDesktopMinimumConnectionSpeed) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeDesktopHardwareRequirementsStatus() =>
            !string.IsNullOrWhiteSpace(NativeDesktopHardwareRequirements) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeDesktopMemoryAndStorageStatus() => NativeDesktopMemoryAndStorage?.Status() ?? TaskProgress.NotStarted;

        public TaskProgress NativeMobileMemoryAndStorageStatus() => MobileMemoryAndStorage?.Status() ?? TaskProgress.NotStarted;

        public TaskProgress NativeDesktopSupportedOperatingSystemsStatus() =>
            !string.IsNullOrWhiteSpace(NativeDesktopOperatingSystemsDescription) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeDesktopThirdPartyStatus() => NativeDesktopThirdParty?.Status() ?? TaskProgress.NotStarted;

        public TaskProgress NativeMobileConnectivityStatus() => MobileConnectionDetails?.Status() ?? TaskProgress.NotStarted;

        public TaskProgress NativeMobileSupportedOperatingSystemsStatus() => MobileOperatingSystems?.Status() ?? TaskProgress.NotStarted;

        public TaskProgress NativeMobileAdditionalInformationStatus() =>
            !string.IsNullOrWhiteSpace(NativeMobileAdditionalInformation) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeMobileHardwareRequirementsStatus() =>
            !string.IsNullOrWhiteSpace(NativeMobileHardwareRequirements) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress PluginsStatus() => (Plugins?.Required != null) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress SupportedBrowsersStatus() =>
            (BrowsersSupported != null && BrowsersSupported.Any() && MobileResponsive.HasValue) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeMobileThirdPartyStatus() => MobileThirdParty?.Status() ?? TaskProgress.NotStarted;

        public TaskProgress ApplicationTypeStatus(ClientApplicationType applicationType)
        {
            if (applicationType == ClientApplicationType.BrowserBased)
            {
                if (SupportedBrowsersStatus() == TaskProgress.Completed && PluginsStatus() == TaskProgress.Completed)
                    return TaskProgress.Completed;

                if (SupportedBrowsersStatus() == TaskProgress.Completed ||
                    PluginsStatus() == TaskProgress.Completed ||
                    ConnectivityStatus() == TaskProgress.Completed ||
                    HardwareRequirementsStatus() == TaskProgress.Completed ||
                    AdditionalInformationStatus() == TaskProgress.Completed)
                {
                    return TaskProgress.InProgress;
                }
            }

            if (applicationType == ClientApplicationType.Desktop)
            {
                if (NativeDesktopSupportedOperatingSystemsStatus() == TaskProgress.Completed &&
                    NativeDesktopConnectivityStatus() == TaskProgress.Completed &&
                    NativeDesktopMemoryAndStorageStatus() == TaskProgress.Completed)
                {
                    return TaskProgress.Completed;
                }

                if (NativeDesktopSupportedOperatingSystemsStatus() == TaskProgress.Completed ||
                    NativeDesktopConnectivityStatus() == TaskProgress.Completed ||
                    NativeDesktopMemoryAndStorageStatus() == TaskProgress.Completed ||
                    NativeDesktopThirdPartyStatus() == TaskProgress.Completed ||
                    NativeDesktopHardwareRequirementsStatus() == TaskProgress.Completed ||
                    NativeDesktopAdditionalInformationStatus() == TaskProgress.Completed)
                {
                    return TaskProgress.InProgress;
                }
            }

            if (applicationType == ClientApplicationType.MobileTablet)
            {
                if (NativeMobileSupportedOperatingSystemsStatus() == TaskProgress.Completed &&
                    NativeMobileMemoryAndStorageStatus() == TaskProgress.Completed)
                {
                    return TaskProgress.Completed;
                }

                if (NativeMobileSupportedOperatingSystemsStatus() == TaskProgress.Completed ||
                    NativeMobileConnectivityStatus() == TaskProgress.Completed ||
                    NativeMobileMemoryAndStorageStatus() == TaskProgress.Completed ||
                    NativeMobileThirdPartyStatus() == TaskProgress.Completed ||
                    NativeMobileHardwareRequirementsStatus() == TaskProgress.Completed ||
                    NativeMobileAdditionalInformationStatus() == TaskProgress.Completed)
                {
                    return TaskProgress.InProgress;
                }
            }

            return TaskProgress.NotStarted;
        }
    }
}
