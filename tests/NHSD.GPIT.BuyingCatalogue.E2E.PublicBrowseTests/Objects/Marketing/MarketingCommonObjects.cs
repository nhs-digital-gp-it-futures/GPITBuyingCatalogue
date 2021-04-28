using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing
{
    internal static class MarketingCommonObjects
    {
        internal static By SaveAndReturn => CustomBy.DataTestId("section-submit-button", "button[type=submit]");

        internal static By GoBackLink => CustomBy.DataTestId("go-back-link", "a");
    }
}
