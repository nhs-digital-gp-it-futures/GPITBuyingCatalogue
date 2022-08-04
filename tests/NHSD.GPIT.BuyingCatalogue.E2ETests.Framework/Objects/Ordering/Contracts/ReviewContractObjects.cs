using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts
{
    public static class ReviewContractObjects
    {
        public static By DownloadPdfButton => By.LinkText("Download order PDF");

        public static By CompleteOrderButton => By.LinkText("Complete order");

        public static By AssociatedServicesExpander => By.Id("associated-services-expander");

        public static By DataProcessingExpander => By.Id("data-processing-expander");

        public static By ImplementationPlanExpander => By.Id("implementation-plan-expander");

        public static By BespokeDataProcessing => By.Id("bespoke-data-processing");

        public static By BespokeImplementationPlan => By.Id("bespoke-implementation-plan");
    }
}
