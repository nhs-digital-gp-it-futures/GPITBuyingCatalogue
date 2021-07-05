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
    }
}
