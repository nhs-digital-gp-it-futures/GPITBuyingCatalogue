using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class ClientApplicationProgress
    {
        private readonly ClientApplication clientApplication;

        public ClientApplicationProgress(ClientApplication clientAppication)
        {
            clientApplication = clientAppication;
        }

        public TaskProgress AdditionalInformationStatus() => Status(clientApplication.AdditionalInformation);

        public TaskProgress ConnectivityStatus() => Status(clientApplication.MinimumConnectionSpeed);

        public TaskProgress HardwareRequirementsStatus() => Status(clientApplication.HardwareRequirements);

        public TaskProgress NativeDesktopAdditionalInformationStatus() => Status(clientApplication.NativeDesktopAdditionalInformation);

        public TaskProgress NativeDesktopConnectivityStatus() => Status(clientApplication.NativeDesktopMinimumConnectionSpeed);

        public TaskProgress NativeDesktopHardwareRequirementsStatus() => Status(clientApplication.NativeDesktopHardwareRequirements);

        public TaskProgress NativeDesktopMemoryAndStorageStatus() => Status(GetStatus(clientApplication.NativeDesktopMemoryAndStorage));

        public TaskProgress NativeMobileMemoryAndStorageStatus() => Status(GetStatus(clientApplication.MobileMemoryAndStorage));

        public TaskProgress NativeDesktopSupportedOperatingSystemsStatus() => Status(clientApplication.NativeDesktopOperatingSystemsDescription);

        public TaskProgress NativeDesktopThirdPartyStatus() => Status(GetStatus(clientApplication.NativeDesktopThirdParty));

        public TaskProgress NativeMobileConnectivityStatus() => Status(GetStatus(clientApplication.MobileConnectionDetails));

        public TaskProgress NativeMobileSupportedOperatingSystemsStatus() => Status(GetStatus(clientApplication.MobileOperatingSystems));

        public TaskProgress NativeMobileAdditionalInformationStatus() => Status(clientApplication.NativeMobileAdditionalInformation);

        public TaskProgress NativeMobileHardwareRequirementsStatus() => Status(clientApplication.NativeMobileHardwareRequirements);

        public TaskProgress PluginsStatus() => (clientApplication.Plugins?.Required != null) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress SupportedBrowsersStatus() =>
            (clientApplication.BrowsersSupported != null && clientApplication.BrowsersSupported.Any() && clientApplication.MobileResponsive.HasValue) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeMobileThirdPartyStatus() => Status(GetStatus(clientApplication.MobileThirdParty));

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

        private static TaskProgress GetStatus(MobileConnectionDetails mobileConnectionDetails)
        {
            if (!string.IsNullOrWhiteSpace(mobileConnectionDetails?.MinimumConnectionSpeed) ||
                !string.IsNullOrWhiteSpace(mobileConnectionDetails?.Description) ||
                (mobileConnectionDetails?.ConnectionType?.Any() ?? false))
            {
                return TaskProgress.Completed;
            }

            return TaskProgress.NotStarted;
        }

        private static TaskProgress GetStatus(MobileMemoryAndStorage mobileMemoryAndStorage)
        {
            if (!string.IsNullOrWhiteSpace(mobileMemoryAndStorage?.Description) && !string.IsNullOrWhiteSpace(mobileMemoryAndStorage?.MinimumMemoryRequirement))
                return TaskProgress.Completed;

            return TaskProgress.NotStarted;
        }

        private static TaskProgress GetStatus(MobileThirdParty mobileThirdParty)
        {
            if (!string.IsNullOrWhiteSpace(mobileThirdParty?.ThirdPartyComponents) ||
                !string.IsNullOrWhiteSpace(mobileThirdParty?.DeviceCapabilities))
            {
                return TaskProgress.Completed;
            }

            return TaskProgress.NotStarted;
        }

        private static TaskProgress GetStatus(NativeDesktopThirdParty nativeDesktopThirdParty)
        {
            if (!string.IsNullOrWhiteSpace(nativeDesktopThirdParty?.DeviceCapabilities) || !string.IsNullOrWhiteSpace(nativeDesktopThirdParty?.ThirdPartyComponents))
                return TaskProgress.Completed;

            return TaskProgress.NotStarted;
        }

        private static TaskProgress GetStatus(NativeDesktopMemoryAndStorage nativeDesktopMemoryAndStorage)
        {
            if (!string.IsNullOrWhiteSpace(nativeDesktopMemoryAndStorage?.MinimumMemoryRequirement)
                && !string.IsNullOrWhiteSpace(nativeDesktopMemoryAndStorage?.StorageRequirementsDescription)
                && !string.IsNullOrWhiteSpace(nativeDesktopMemoryAndStorage?.MinimumCpu))
            {
                return TaskProgress.Completed;
            }

            return TaskProgress.NotStarted;
        }

        private static TaskProgress GetStatus(MobileOperatingSystems mobileOperatingSystems)
        {
            return (mobileOperatingSystems?.OperatingSystems?.Any() ?? false) ? TaskProgress.Completed : TaskProgress.NotStarted;
        }

        private static TaskProgress Status(string value) => string.IsNullOrWhiteSpace(value)
            ? TaskProgress.NotStarted
            : TaskProgress.Completed;

        private static TaskProgress Status(TaskProgress? progress) => progress ?? TaskProgress.NotStarted;
    }
}
