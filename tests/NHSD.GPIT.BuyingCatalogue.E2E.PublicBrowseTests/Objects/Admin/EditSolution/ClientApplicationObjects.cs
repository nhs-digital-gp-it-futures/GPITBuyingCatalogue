using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.EditSolution
{
    internal static class ClientApplicationObjects
    {
        internal static By ThirdPartyComponents => By.Id("ThirdPartyComponents");

        internal static By DeviceCapabilities => By.Id("DeviceCapabilities");

        internal static By MinimumMemoryDropDown => By.Id("SelectedMemorySize");

        internal static By ProcessingPower => By.Id("ProcessingPower");

        internal static By ResolutionDropdown => By.Id("SelectedResolution");

        internal static By StorageSpace => By.Id("StorageSpace");
    }
}
