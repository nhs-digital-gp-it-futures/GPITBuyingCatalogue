﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class OrderSummaryObjects
    {
        public static By ReviewAndCompleteOrderLink => By.LinkText("Review and complete order");

        public static By OrderIdSummary => ByExtensions.DataTestId("order-id-summary");

        public static By AmendThisOrder => By.ClassName("nhsuk-action-link__text");

        public static By OrderDescriptionSummary => ByExtensions.DataTestId("order-description-summary");

        public static By DateCreatedSummary => ByExtensions.DataTestId("date-created-summary");

        public static By OrderingPartySummary => ByExtensions.DataTestId("ordering-party-summary");

        public static By SupplierSummary => ByExtensions.DataTestId("supplier-summary");

        public static By SupplierLegalName => ByExtensions.DataTestId("supplier-legal-name"); 
        
        public static By StartDateSummary => ByExtensions.DataTestId("start-date-summary");

        public static By InitialPeriodSummary => ByExtensions.DataTestId("initial-period-summary");

        public static By MaximumTermSummary => ByExtensions.DataTestId("maximum-term-summary");

        public static By EndDateSummary => ByExtensions.DataTestId("end-date-summary");

        public static By SolutionSection => By.Id("catalogue-solutions-title");

        public static By AdditionalServicesSection => By.Id("additional-services-title");

        public static By AssociatedServicesSection => By.Id("associated-services-title");
        public static By IndicativeCostsSection => By.Id("associated-services-title");

        public static By OneOffCostSummary => ByExtensions.DataTestId("one-off-cost-summary");

        public static By MonthlyCostSummary => ByExtensions.DataTestId("monthly-cost-summary");

        public static By OneYearCostSummary => ByExtensions.DataTestId("one-year-cost-summary");

        public static By TotalCostSummary => ByExtensions.DataTestId("total-cost-summary");

        public static By DownloadPdfButton => By.LinkText("Download order form (PDF)");

        public static By CompleteOrderButton => By.LinkText("Complete order");

        public static By SaveForLaterButton => By.LinkText("Save for later");

        public static By AssociatedServicesExpander => By.Id("associated-services-expander");

        public static By DataProcessingExpander => By.Id("data-processing-expander");

        public static By ImplementationPlanExpander => By.Id("implementation-plan-expander");

        public static By BespokeDataProcessing => By.Id("bespoke-data-processing");

        public static By BespokeImplementationPlan => By.Id("bespoke-implementation-plan");
    }
}
