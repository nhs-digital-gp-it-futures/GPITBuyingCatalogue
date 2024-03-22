using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class ImportObjects
    {
        public static By CsvUrlInput => By.Id("CsvUrl");

        public static By CsvUrlError => By.Id("CsvUrl-error");

        public static By ImportCsvFileInput => By.Id("File");
    }
}
