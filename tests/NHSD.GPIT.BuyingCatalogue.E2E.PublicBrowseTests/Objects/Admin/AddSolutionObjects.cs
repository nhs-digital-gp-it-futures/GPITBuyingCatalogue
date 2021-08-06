﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class AddSolutionObjects
    {
        internal static By SolutionName => By.Id("SolutionName");

        internal static By SupplierName => By.Id("SupplierId");

        internal static By SolutionFrameworks => ByExtensions.DataTestId("framework-names");

        internal static By SaveSolutionButton => By.Id("Submit");

        internal static By FoundationSolution => ByExtensions.DataTestId("foundation-solution");

        internal static By ManageSuppliersOrgsLink => By.LinkText("Manage suppliers organisations");

        internal static By AddSuppliersOrgLink => By.CssSelector(".nhsuk-action-link a");

        internal static By SupplierOrgsTable => ByExtensions.DataTestId("manage-suppliers-table");

        internal static By SupplierOrgRow => ByExtensions.DataTestId("manage-suppliers-table");

        internal static By SupplierEditLink => ByExtensions.DataTestId("edit-link");

        internal static By AddSolutionLink => By.LinkText("Add a solution");

        internal static By CatalogueSolutionLink => By.LinkText("Manage Catalogue Solutions");

        internal static By CatalogueSolutionTable => ByExtensions.DataTestId("manage-catalogue-solution-list");

        internal static By CatalogueSolutionFilter => By.ClassName("nhsuk-details");

        internal static By FilterRadioButton => By.ClassName("nhsuk-radios__item");

        internal static By ConnectivityDropdown => By.Id("SelectedConnectionSpeed");

        internal static By ResolutionDropdown => By.Id("SelectedScreenResolution");
    }
}
