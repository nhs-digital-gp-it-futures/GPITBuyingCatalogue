using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class OrderTriageObjects
    {
        public static By ProcurementHubLink => By.Id("procurement-hub-link");

        public static By ProcurementHubActionLink => By.LinkText("Contact Procurement Hub");

        public static By FundingInset => ByExtensions.DataTestId("funding-inset");

        public static By FundingInsetLink => By.CssSelector(".nhsuk-details__summary-text");

        public static By FundingInsetText => By.Id("nhsuk-details__text0");

        public static By FundingSource => By.Id("selected-funding-source");

        public static By FundingSourceError => By.Id("selected-funding-source-error");
    }
}
