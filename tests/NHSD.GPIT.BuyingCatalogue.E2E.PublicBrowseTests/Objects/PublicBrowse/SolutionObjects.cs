using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionObjects
    {
        internal static By ImplementationName => By.TagName("h1");

        internal static By SolutionName => ByExtensions.DataTestId("view-solution-page-solution-name");

        internal static By SolutionDetailTableRow => By.ClassName("nhsuk-summary-list__row");

        internal static By SummaryAndDescription => By.TagName("p");

        internal static By NhsSolutionEpics => ByExtensions.DataTestId("nhs-defined-epics", "li");

        internal static By SupplierSolutionEpics => ByExtensions.DataTestId("supplier-defined-epics", "li");

        internal static By CapabilitiesContent => By.CssSelector("tbody tr td:nth-child(1)");

        internal static By FlatPriceTable => ByExtensions.DataTestId("flat-list-price-table");

        internal static By PriceColumn => ByExtensions.DataTestId("price");

        internal static By CheckEpicLink => ByExtensions.DataTestId("check-epics-link");

        internal static By AssociatedServicesTable => ByExtensions.DataTestId("associated-services-table");

        internal static By AssociatedServicesInformation => ByExtensions.DataTestId("associated-services-details");

        internal static By Description => ByExtensions.DataTestId("description");

        internal static By OrderGuidance => ByExtensions.DataTestId("order-guidance");

        internal static By AdditionalServicesTable => ByExtensions.DataTestId("additional-services-table");

        internal static By FullDescription => ByExtensions.DataTestId("full-description");

        internal static By BreadcrumbsBanner => By.ClassName("nhsuk-breadcrumb__item");

        internal static By CatalogueSolutionCrumb => ByExtensions.DataTestId("catalogue-solutions-crumb");

        internal static By CatalogueSolutionPage => ByExtensions.DataTestId("solutions-list-body");

        internal static By CapabilityName => ByExtensions.DataTestId("capability-name");

        internal static By InRemediationNotice => By.ClassName("nhsuk-warning-callout");

        internal static By SolutionSuspendedNotice => ByExtensions.DataTestId("solution-suspended-notice");

        internal static By SolutionNavigationMenu => By.ClassName("nhsuk-contents-list");
    }
}
