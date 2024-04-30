using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Framework;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Gen2;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.HostingType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSupplier;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageUsers;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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
            Interoperability = new Interoperability(driver, commonActions);
            Implementation = new Implementation(driver, commonActions);
            SolutionApplicationTypes = new SolutionApplicationTypes(driver, commonActions);
            BrowserBased = new BrowserBased(driver, commonActions);
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

        internal Interoperability Interoperability { get; }

        internal Implementation Implementation { get; }

        internal SolutionApplicationTypes SolutionApplicationTypes { get; }

        internal BrowserBased BrowserBased { get; }

        public void AddSolutionDetailsAndDescription()
        {
            AddNewSolution.AddSolution();
            AddNewSolution.AddSolutionDetails();

            var solutionId = GetSolutionID();
            AddNewSolution.AddSolutionDescription(solutionId);
            Features.AddSolutionFeature(solutionId);
        }

        public void AddSolutionInteroperability(ProviderOrConsumer providerOrConsumer)
        {
            var solutionId = GetSolutionID();
            Interoperability.AddInteroperability(solutionId);
            Interoperability.AddIM1Integrations(providerOrConsumer);
            Interoperability.AddGPConnect1Integrations(providerOrConsumer);
            Interoperability.AddNHSAppIntegrations();
        }

        public void AddSolutionImplementation()
        {
            var solutionId = GetSolutionID();
            Implementation.AddImpementation(solutionId);
            Implementation.AddImplementationDetails();
        }

        public void AddSolutionApplicationTypes()
        {
            var solutionId = GetSolutionID();
            SolutionApplicationTypes.AddApplicationType(solutionId);
            BrowserBased.AddBrowserBasedApplication();
            BrowserBased.AddBrowserBasedApplicationTypes();
        }

        private string GetSolutionID()
        {
            using var dbContext = Factory.DbContext;

            var solution = Driver.Url.Split('/').Last();
            return solution;
        }
    }
}
