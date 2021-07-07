using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class AddSolutionObjects
    {
        internal static By SolutionName => By.Id("SolutionName");

        internal static By SupplierName => By.Id("SupplierId");

        internal static By SolutionFrameworks => CustomBy.DataTestId("framework-names");

        internal static By SaveSolutionButton => By.Id("Submit");

        internal static By FoundationSolution => CustomBy.DataTestId("foundation-solution");

        internal static By AddSolutionLink => By.LinkText("Add a solution");

        internal static By CatalogueSolutionLink => By.LinkText("Manage Catalogue Solutions");

        internal static By CatalogueSolutionTable => CustomBy.DataTestId("manage-catalogue-solution-list");

        internal static By CatalogueSolutionFilter => By.Id("nhsuk-details0");

        internal static By FilterRadioButton => By.ClassName("nhsuk-radios__item");
    }
}
