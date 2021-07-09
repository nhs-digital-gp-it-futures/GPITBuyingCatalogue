using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionObjects
    {
        internal static By ImplementationName => By.TagName("h1");

        internal static By SolutionName => CustomBy.DataTestId("view-solution-page-solution-name");

        internal static By SolutionDetailTableRow => By.ClassName("nhsuk-summary-list__row");

        internal static By SummaryAndDescription => By.TagName("p");

        internal static By NhsSolutionEpics => CustomBy.DataTestId("nhs-defined-epics", "li");

        internal static By SupplierSolutionEpics => CustomBy.DataTestId("supplier-defined-epics", "li");

        internal static By CapabilitiesContent => By.CssSelector("tbody tr td:nth-child(1)");

        internal static By FlatPriceTable => CustomBy.DataTestId("flat-list-price-table");

        internal static By PriceColumn => CustomBy.DataTestId("price");

        internal static By SolutionEpicLink => By.LinkText("Check Epics");

        internal static By AssociatedServicesTable => CustomBy.DataTestId("associated-services-table");

        internal static By AssociatedServicesInformation => CustomBy.DataTestId("associated-services-details");

        internal static By Description => CustomBy.DataTestId("description");

        internal static By OrderGuidance => CustomBy.DataTestId("order-guidance");

        internal static By AdditionalServicesTable => CustomBy.DataTestId("additional-services-table");

        internal static By FullDescription => CustomBy.DataTestId("full-description");
        
        internal static By BreadcrumbsBanner => By.ClassName("nhsuk-breadcrumb__item");

        internal static By CatalogueSolutionCrumb => CustomBy.DataTestId("catalogue-solutions-crumb");

        internal static By CatalogueSolutionPage => CustomBy.DataTestId("solutions-list-body");
    }
}
