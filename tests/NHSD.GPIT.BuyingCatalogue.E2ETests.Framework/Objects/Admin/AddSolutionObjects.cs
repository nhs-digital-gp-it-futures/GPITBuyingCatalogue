using FluentAssertions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class AddSolutionObjects
    {
        public static By SolutionName => By.Id("SolutionName");

        public static By SupplierName => By.Id("SupplierId");

        public static By SolutionFrameworks => ByExtensions.DataTestId("framework-names");

        public static By SaveSolutionButton => By.Id("Submit");

        public static By FoundationSolution => ByExtensions.DataTestId("foundation-solution");

        public static By ManageSuppliersOrgsLink => By.LinkText("Manage suppliers organisations");

        public static By AddSuppliersOrgLink => By.CssSelector(".nhsuk-action-link a");

        public static By SupplierOrgsTable => ByExtensions.DataTestId("manage-suppliers-table");

        public static By SupplierOrgRow => ByExtensions.DataTestId("manage-suppliers-table");

        public static By SupplierEditLink => ByExtensions.DataTestId("edit-link");

        public static By AddSolutionLink => By.LinkText("Add a solution");

        public static By CatalogueSolutionLink => By.LinkText("Manage Catalogue Solutions");

        public static By CatalogueSolutionTable => ByExtensions.DataTestId("manage-catalogue-solution-list");

        public static By CatalogueSolutionFilter => By.ClassName("nhsuk-details");

        public static By FilterRadioButton => By.ClassName("nhsuk-radios__item");

        public static By ConnectivityDropdown => By.Id("SelectedConnectionSpeed");

        public static By ResolutionDropdown => By.Id("SelectedScreenResolution");

        public static By SolutionDescriptionLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/description')]");

        public static By SolutionFeatureLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/features')]");

        public static By ListPriceLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/list-prices')]");

        public static By SolutionInteroperabilityLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/interoperability')]");

        public static By SolutionImplementationLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/implementation')]");

        public static By SolutionApplicationTypeLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/application-type')]");

        public static By SolutionHostingTypeLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/hosting-type')]");

        public static By CapabilitiesAndEpicsLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/edit-capabilities')]");

        public static By AdditionalServiceLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/additional-services')]");

        public static By AssociatedServiceLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/associated-services')]");

        public static By DevelopmentPlansLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/development-plans')]");

        public static By SupplierDetailsLink(string solutionId) => By.XPath($"//a[contains(@href, '/manage/" + solutionId + "/supplier-details')]");

        public static By AddApplicationTypeLink => By.LinkText("Add an application type");

        public static By AddHostingTypeLink => By.LinkText("Add a hosting type");
    }
}
