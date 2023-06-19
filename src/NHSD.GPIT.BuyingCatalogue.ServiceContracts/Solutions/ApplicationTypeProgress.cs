using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class ApplicationTypeProgress
    {
        private readonly ApplicationTypeDetail appicationTypeDetail;

        public ApplicationTypeProgress(ApplicationTypeDetail appicationTypeDetail)
        {
            this.appicationTypeDetail = appicationTypeDetail;
        }

        public TaskProgress AdditionalInformationStatus() => Status(appicationTypeDetail.AdditionalInformation);

        public TaskProgress ConnectivityStatus() => Status(appicationTypeDetail.MinimumConnectionSpeed);

        public TaskProgress HardwareRequirementsStatus() => Status(appicationTypeDetail.HardwareRequirements);

        public TaskProgress NativeDesktopAdditionalInformationStatus() => Status(appicationTypeDetail.NativeDesktopAdditionalInformation);

        public TaskProgress NativeDesktopConnectivityStatus() => Status(appicationTypeDetail.NativeDesktopMinimumConnectionSpeed);

        public TaskProgress NativeDesktopHardwareRequirementsStatus() => Status(appicationTypeDetail.NativeDesktopHardwareRequirements);

        public TaskProgress NativeDesktopMemoryAndStorageStatus() => Status(GetStatus(appicationTypeDetail.NativeDesktopMemoryAndStorage));

        public TaskProgress NativeMobileMemoryAndStorageStatus() => Status(GetStatus(appicationTypeDetail.MobileMemoryAndStorage));

        public TaskProgress NativeDesktopSupportedOperatingSystemsStatus() => Status(appicationTypeDetail.NativeDesktopOperatingSystemsDescription);

        public TaskProgress NativeDesktopThirdPartyStatus() => Status(GetStatus(appicationTypeDetail.NativeDesktopThirdParty));

        public TaskProgress NativeMobileConnectivityStatus() => Status(GetStatus(appicationTypeDetail.MobileConnectionDetails));

        public TaskProgress NativeMobileSupportedOperatingSystemsStatus() => Status(GetStatus(appicationTypeDetail.MobileOperatingSystems));

        public TaskProgress NativeMobileAdditionalInformationStatus() => Status(appicationTypeDetail.NativeMobileAdditionalInformation);

        public TaskProgress NativeMobileHardwareRequirementsStatus() => Status(appicationTypeDetail.NativeMobileHardwareRequirements);

        public TaskProgress PluginsStatus() => (appicationTypeDetail.Plugins?.Required != null) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress SupportedBrowsersStatus() =>
            (appicationTypeDetail.BrowsersSupported != null && appicationTypeDetail.BrowsersSupported.Any() && appicationTypeDetail.MobileResponsive.HasValue) ? TaskProgress.Completed : TaskProgress.NotStarted;

        public TaskProgress NativeMobileThirdPartyStatus() => Status(GetStatus(appicationTypeDetail.MobileThirdParty));

        public TaskProgress ApplicationTypeStatus(ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.BrowserBased)
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

            if (applicationType == ApplicationType.Desktop)
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

            if (applicationType == ApplicationType.MobileTablet)
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
