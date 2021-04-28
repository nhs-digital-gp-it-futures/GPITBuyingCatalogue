using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing
{
    internal static class SolutionDescriptionObjects
    {
        internal static By SaveAndReturn => CustomBy.DataTestId("section-submit-button", "button[type=submit]");

        internal static By Description => By.Id("description");

        internal static By Summary => By.Id("summary");

        internal static By Link => By.Id("link");
    }
}
