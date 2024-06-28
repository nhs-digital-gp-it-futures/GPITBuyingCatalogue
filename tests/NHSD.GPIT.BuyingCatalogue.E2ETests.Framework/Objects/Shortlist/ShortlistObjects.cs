using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Shortlist
{
    public static class ShortlistObjects
    {
        public static By CreateNewShortlist => By.LinkText("Create new shortlist");

        public static By FilterByFoundationCapabilitiesLink => By.LinkText("Filter by Foundation Capabilities");

        public static By FitlerName => By.Id("Name");

        public static By FilterDescription => By.Id("Description");
    }
}
