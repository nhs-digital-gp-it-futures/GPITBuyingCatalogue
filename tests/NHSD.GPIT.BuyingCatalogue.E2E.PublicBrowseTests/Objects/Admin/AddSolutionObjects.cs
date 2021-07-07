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

        internal static By ManageSuppliersOrgsLink => By.LinkText("Manage suppliers organisations");

        internal static By AddSuppliersOrgLink => By.LinkText("Add a supplier");

        internal static By SupplierOrgsTable => By.ClassName("nhsuk-form-group");

        internal static By SupplierOrgRow => CustomBy.DataTestId("manage-suppliers-table");

        internal static By SupplierEditLink => By.LinkText("Edit");
    }
}
