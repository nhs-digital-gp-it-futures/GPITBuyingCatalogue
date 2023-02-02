using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class AdvancedTelephonyObjects
    {
        public static By HomepageButton => By.LinkText("Back to homepage");

        public static By DownloadComissioningSupportPackPDFButton => By.LinkText("Download buyer’s guide (PDF)");

    }
}
