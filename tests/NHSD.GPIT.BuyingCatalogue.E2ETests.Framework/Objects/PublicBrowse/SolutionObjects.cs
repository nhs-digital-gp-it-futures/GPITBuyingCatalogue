using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class SolutionObjects
    {
        public static By ImplementationName => By.TagName("h1");

        public static By SolutionName => ByExtensions.DataTestId("view-solution-page-solution-name");

        public static By SolutionDetailTableRow => By.ClassName("nhsuk-summary-list__row");

        public static By SummaryAndDescription => By.TagName("p");

        public static By NhsSolutionEpics => ByExtensions.DataTestId("nhs-defined-epics", "li");

        public static By SupplierSolutionEpics => ByExtensions.DataTestId("supplier-defined-epic-name");

        public static By CapabilitiesContent => By.CssSelector("tbody tr td:nth-child(1)");

        public static By PriceColumn => ByExtensions.DataTestId("price");

        public static By CheckEpicLink => ByExtensions.DataTestId("check-epics-link");

        public static By AssociatedServicesTieredTable => ByExtensions.DataTestId("associated-services-tiered-table");

        public static By AssociatedServicesFlatTable => ByExtensions.DataTestId("associated-services-flat-table");

        public static By AssociatedServicesInformation => ByExtensions.DataTestId("associated-services-details");

        public static By Description => ByExtensions.DataTestId("description");

        public static By OrderGuidance => ByExtensions.DataTestId("order-guidance");

        public static By AdditionalServicesTieredTable => ByExtensions.DataTestId("additional-services-tiered-table");

        public static By AdditionalServicesFlatTable => ByExtensions.DataTestId("additional-services-flat-table");

        public static By FullDescription => ByExtensions.DataTestId("description");

        public static By BreadcrumbsBanner => By.ClassName("nhsuk-breadcrumb__item");

        public static By CatalogueSolutionCrumb => ByExtensions.DataTestId("catalogue-solutions-crumb");

        public static By CatalogueSolutionPage => ByExtensions.DataTestId("solutions-list-body");

        public static By CapabilityName => ByExtensions.DataTestId("capability-name");

        public static By PilotWarningCallout => ByExtensions.DataTestId("pilot-warning-callout");

        public static By InRemediationNotice => ByExtensions.DataTestId("in-remediation-warning-callout");

        public static By SolutionSuspendedNotice => ByExtensions.DataTestId("solution-suspended-notice");

        public static By SolutionNavigationMenu => By.ClassName("nhsuk-contents-list");
    }
}
