using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class CommonObjects
    {
        internal static By ContinueButton => By.CssSelector("button[type=submit]");

        internal static By PageTitle => By.TagName("h1");

        internal static By GoBackLink => CustomBy.DataTestId("go-back-link", "a");
    }
}
