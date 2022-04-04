using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class ImportObjects
    {
        internal static By CsvUrlInput => By.Id("CsvUrl");

        internal static By CsvUrlError => By.Id("CsvUrl-error");
    }
}
