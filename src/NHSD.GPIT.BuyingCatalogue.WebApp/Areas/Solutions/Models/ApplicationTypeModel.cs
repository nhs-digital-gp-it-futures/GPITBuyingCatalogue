using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ApplicationTypeModel
    {
        public ApplicationTypeModel(ApplicationType type, ApplicationTypes client)
        {
            if (client == null)
            {
                DisplayApplicationType = false;
                return;
            }

            switch (type)
            {
                case ApplicationType.BrowserBased:
                    LoadBrowserBasedDetails(client);
                    break;

                case ApplicationType.NativeMobile:
                    LoadNativeMobileDetails(client);
                    break;

                case ApplicationType.NativeDesktop:
                    LoadNativeDesktopDetails(client);
                    break;
            }
        }

        public enum ApplicationType
        {
            BrowserBased,
            NativeMobile,
            NativeDesktop,
        }

        public bool DisplayApplicationType { get; set; }

        public string Label { get; set; }

        public string DataTestTag { get; set; }

        public HashSet<SupportedBrowser> BrowsersSupported { get; set; } = new();

        public bool? MobileResponsive { get; set; }

        public bool? MobileFirstDesign { get; set; }

        public bool? PluginsRequired { get; set; }

        public string PluginDetails { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public string ScreenResolution { get; set; }

        public string HardwareRequirements { get; set; }

        public string AdditionalInformation { get; set; }

        public HashSet<string> MobileOperatingSystems { get; set; } = new();

        public string OperatingSystemDescription { get; set; }

        public HashSet<string> MobileConnectionTypes { get; set; } = new();

        public string ConnectionRequirements { get; set; }

        public string MemoryRequirements { get; set; }

        public string StorageSpace { get; set; }

        public string ThirdPartyComponents { get; set; }

        public string DeviceCapabilities { get; set; }

        public string ProcessingPower { get; set; }

        private void LoadBrowserBasedDetails(ApplicationTypes client)
        {
            Label = "Browser-based application";
            DataTestTag = "browser-based";
            BrowsersSupported = client.BrowsersSupported;
            MobileResponsive = client.MobileResponsive;
            MobileFirstDesign = client.MobileFirstDesign;
            PluginsRequired = client.Plugins?.Required;
            PluginDetails = client.Plugins?.AdditionalInformation;
            MinimumConnectionSpeed = client.MinimumConnectionSpeed;
            ScreenResolution = client.MinimumDesktopResolution;
            HardwareRequirements = client.HardwareRequirements;
            AdditionalInformation = client.AdditionalInformation;

            DisplayApplicationType = BrowsersSupported.Any() ||
                MobileResponsive.HasValue ||
                MobileFirstDesign.HasValue ||
                PluginsRequired.HasValue ||
                !string.IsNullOrWhiteSpace(PluginDetails) ||
                !string.IsNullOrWhiteSpace(MinimumConnectionSpeed) ||
                !string.IsNullOrWhiteSpace(ScreenResolution) ||
                !string.IsNullOrWhiteSpace(HardwareRequirements) ||
                !string.IsNullOrWhiteSpace(AdditionalInformation);
        }

        private void LoadNativeMobileDetails(ApplicationTypes client)
        {
            Label = "Native mobile or tablet application";
            DataTestTag = "native-mobile";
            MobileOperatingSystems = client.MobileOperatingSystems?.OperatingSystems ?? new HashSet<string>();
            OperatingSystemDescription = client.MobileOperatingSystems?.OperatingSystemsDescription;
            MinimumConnectionSpeed = client.MobileConnectionDetails?.MinimumConnectionSpeed;
            MobileConnectionTypes = client.MobileConnectionDetails?.ConnectionType ?? new HashSet<string>();
            ConnectionRequirements = client.MobileConnectionDetails?.Description;
            MemoryRequirements = client.MobileMemoryAndStorage?.MinimumMemoryRequirement;
            StorageSpace = client.MobileMemoryAndStorage?.Description;
            ThirdPartyComponents = client.MobileThirdParty?.ThirdPartyComponents;
            DeviceCapabilities = client.MobileThirdParty?.DeviceCapabilities;
            HardwareRequirements = client.NativeMobileHardwareRequirements;
            AdditionalInformation = client.NativeMobileAdditionalInformation;

            DisplayApplicationType = MobileOperatingSystems.Any() ||
                !string.IsNullOrWhiteSpace(OperatingSystemDescription) ||
                !string.IsNullOrWhiteSpace(MinimumConnectionSpeed) ||
                MobileConnectionTypes.Any() ||
                !string.IsNullOrWhiteSpace(ConnectionRequirements) ||
                !string.IsNullOrWhiteSpace(MemoryRequirements) ||
                !string.IsNullOrWhiteSpace(StorageSpace) ||
                !string.IsNullOrWhiteSpace(ThirdPartyComponents) ||
                !string.IsNullOrWhiteSpace(DeviceCapabilities) ||
                !string.IsNullOrWhiteSpace(HardwareRequirements) ||
                !string.IsNullOrWhiteSpace(AdditionalInformation);
        }

        private void LoadNativeDesktopDetails(ApplicationTypes client)
        {
            Label = "Native desktop application";
            DataTestTag = "native-desktop";
            OperatingSystemDescription = client.NativeDesktopOperatingSystemsDescription;
            MinimumConnectionSpeed = client.NativeDesktopMinimumConnectionSpeed;
            MemoryRequirements = client.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement;
            StorageSpace = client.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription;
            ProcessingPower = client.NativeDesktopMemoryAndStorage?.MinimumCpu;
            ScreenResolution = client.NativeDesktopMemoryAndStorage?.RecommendedResolution;
            ThirdPartyComponents = client.NativeDesktopThirdParty?.ThirdPartyComponents;
            DeviceCapabilities = client.NativeDesktopThirdParty?.DeviceCapabilities;
            HardwareRequirements = client.NativeDesktopHardwareRequirements;
            AdditionalInformation = client.NativeDesktopAdditionalInformation;

            DisplayApplicationType =
                !string.IsNullOrWhiteSpace(OperatingSystemDescription) ||
                !string.IsNullOrWhiteSpace(MinimumConnectionSpeed) ||
                !string.IsNullOrWhiteSpace(MemoryRequirements) ||
                !string.IsNullOrWhiteSpace(StorageSpace) ||
                !string.IsNullOrWhiteSpace(ProcessingPower) ||
                !string.IsNullOrWhiteSpace(ScreenResolution) ||
                !string.IsNullOrWhiteSpace(ThirdPartyComponents) ||
                !string.IsNullOrWhiteSpace(DeviceCapabilities) ||
                !string.IsNullOrWhiteSpace(HardwareRequirements) ||
                !string.IsNullOrWhiteSpace(AdditionalInformation);
        }
    }
}
