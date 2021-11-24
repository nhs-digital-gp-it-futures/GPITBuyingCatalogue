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

        public IReadOnlyList<ClientApplicationType> ExistingClientApplicationTypes
        {
            get
            {
                var result = new List<ClientApplicationType>(3);

                foreach (ClientApplicationType clientApplicationType in Enum.GetValues(typeof(ClientApplicationType)))
                {
                    if (ClientApplicationTypes?.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)) ?? false)
                        result.Add(clientApplicationType);
                }

                return result;
            }
        }

        public void EnsureClientApplicationTypePresent(ClientApplicationType clientApplicationType)
        {
            ClientApplicationTypes ??= new HashSet<string>();

            if (!ClientApplicationTypes.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)))
                ClientApplicationTypes.Add(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        public bool HasClientApplicationType(ClientApplicationType clientApplicationType)
            => ClientApplicationTypes?.Any(type => type.Equals(clientApplicationType.AsString(EnumFormat.EnumMemberValue), StringComparison.OrdinalIgnoreCase)) ?? false;

        public TaskProgress AdditionalInformationStatus() => Status(AdditionalInformation);

        public TaskProgress ConnectivityStatus() => Status(MinimumConnectionSpeed);

        public TaskProgress HardwareRequirementsStatus() => Status(HardwareRequirements);

        public TaskProgress NativeDesktopAdditionalInformationStatus() => Status(NativeDesktopAdditionalInformation);

        public TaskProgress NativeDesktopConnectivityStatus() => Status(NativeDesktopMinimumConnectionSpeed);

        public TaskProgress NativeDesktopHardwareRequirementsStatus() => Status(NativeDesktopHardwareRequirements);

        public TaskProgress NativeDesktopMemoryAndStorageStatus() => Status(NativeDesktopMemoryAndStorage?.Status());

        public TaskProgress NativeMobileMemoryAndStorageStatus() => Status(MobileMemoryAndStorage?.Status());

        public TaskProgress NativeDesktopSupportedOperatingSystemsStatus() => Status(NativeDesktopOperatingSystemsDescription);

        public TaskProgress NativeDesktopThirdPartyStatus() => Status(NativeDesktopThirdParty?.Status());

        public TaskProgress NativeMobileConnectivityStatus() => Status(MobileConnectionDetails?.Status());

        public TaskProgress NativeMobileSupportedOperatingSystemsStatus() => Status(MobileOperatingSystems?.Status());

        public TaskProgress NativeMobileAdditionalInformationStatus() => Status(NativeMobileAdditionalInformation);

        public TaskProgress NativeMobileHardwareRequirementsStatus() => Status(NativeMobileHardwareRequirements);

        public TaskProgress PluginsStatus() => (Plugins?.Required != null) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress SupportedBrowsersStatus() =>
            (BrowsersSupported != null && BrowsersSupported.Any() && MobileResponsive.HasValue) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeMobileThirdPartyStatus() => Status(MobileThirdParty?.Status());

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

        private static TaskProgress Status(string value) => string.IsNullOrWhiteSpace(value)
            ? TaskProgress.NotStarted
            : TaskProgress.Completed;

        private static TaskProgress Status(TaskProgress? progress) => progress ?? TaskProgress.NotStarted;
    }
}
