using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class OrderCompletedObjects
    {
        public static By DownloadPdfButton => By.LinkText("Download order form (PDF)");

        public static By ReturnToDashboardButton => By.LinkText("Return to orders dashboard");

        public static By ContactProcurementLink => By.LinkText("Get procurement support");

        public static By SupportingDocuments => By.Id("supporting-documents");

        public static By HasBespokeBilling => By.Id("has-bespoke-billing");

        public static By HasBespokeDataProcessing => By.Id("has-bespoke-data-processing");

        public static By HasBespokeImplementationPlan => By.Id("has-bespoke-implementation-plan");
    }
}
