using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EditSolution
{
    public static class ClientApplicationObjects
    {
        public static By ThirdPartyComponents => By.Id("ThirdPartyComponents");

        public static By DeviceCapabilities => By.Id("DeviceCapabilities");

        public static By MinimumMemoryDropDown => By.Id("SelectedMemorySize");

        public static By ProcessingPower => By.Id("ProcessingPower");

        public static By ResolutionDropdown => By.Id("SelectedResolution");

        public static By StorageSpace => By.Id("StorageSpace");

        public static By DeleteClientApplicationLink => By.LinkText("Delete application type");

        public static By DeleteClientApplicationCancelLink => By.LinkText("Cancel");
    }
}
