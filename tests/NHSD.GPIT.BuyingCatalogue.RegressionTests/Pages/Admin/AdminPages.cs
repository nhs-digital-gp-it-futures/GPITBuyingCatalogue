//using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Framework;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Gen2;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSupplier;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageUsers;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.SupplierDefinedEpics;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin
{
    public class AdminPages
    {
        private const int OrganisationId = 25;

        public AdminPages(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
        {
            AdminDashboard = new AdminDashboard(driver, commonActions);
            CapabilitiesAndEpicsMappings = new CapabilitiesAndEpicsMappings(driver, commonActions);
            AddFramework = new AddFramework(driver, commonActions);
            AddSupplierDefinedEpics = new AddSupplierDefinedEpics(driver, commonActions);
            AddOrganisationUser = new AddOrganisationUser(driver, commonActions, factory);
            ManageOrganisationSupplier = new ManageOrganisationSupplier(driver, commonActions);
            AddNewSolution = new AddNewSolution(driver, commonActions, factory);
            Features = new Features(driver, commonActions);
            Factory = factory;
            Driver = driver;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

        internal IWebDriver Driver { get; }

        internal AdminDashboard AdminDashboard { get; }

        internal CapabilitiesAndEpicsMappings CapabilitiesAndEpicsMappings { get; }

        internal AddFramework AddFramework { get; }

        internal AddSupplierDefinedEpics AddSupplierDefinedEpics { get; }

        internal AddOrganisationUser AddOrganisationUser { get; }

        internal ManageOrganisationSupplier ManageOrganisationSupplier { get; }

        internal AddNewSolution AddNewSolution { get; }

        internal Features Features { get; }

        public void AddSolutionDetailsAndDescription()
        {
            AddNewSolution.AddSolution();
            AddNewSolution.AddSolutionDetails();

            var solutionId = GetSolutionID();
            AddNewSolution.AddSolutionDescription(solutionId);
            Features.AddSolutionFeature(solutionId);
        }

        private string GetSolutionID()
        {
            using var dbContext = Factory.DbContext;

            var solution = Driver.Url.Split('/').Last();
            return solution;
        }
    }
}
