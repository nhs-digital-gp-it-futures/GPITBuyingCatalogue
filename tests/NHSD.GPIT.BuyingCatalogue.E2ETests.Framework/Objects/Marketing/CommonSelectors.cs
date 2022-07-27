using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing
{
    public static class CommonSelectors
    {
        public static By Description => By.Id("Description");

        public static By Summary => By.Id("Summary");

        public static By Checkbox => By.CssSelector("input[type=checkbox]");

        public static By NhsInput => By.ClassName("nhsuk-input");

        public static By BrowserBasedCheckbox => By.Id("BrowserBased");

        public static By NativeMobileCheckbox => By.Id("NativeMobile");

        public static By NativeDesktopCheckbox => By.Id("NativeDesktop");

        public static By ConnectionSpeedSelect => By.Id("SelectedConnectionSpeed");

        public static By ResolutionSelect => By.Id("SelectedScreenResolution");

        public static By MemorySelect => By.Id("SelectedMemorySize");

        public static By ThirdPartyComponentTextArea => By.Id("ThirdPartyComponents");

        public static By DeviceCapabilityTextArea => By.Id("DeviceCapabilities");

        public static By SupportedOperatingSystemDescription => By.Id("OperatingSystemsDescription");

        public static By StorageDescriptionTextArea => By.Id("StorageDescription");

        public static By MinimumCpuTextArea => By.Id("MinimumCpu");

        public static By PluginsOrExtensionsError => By.Id("plug-ins-or-extensions-error");
    }
}
