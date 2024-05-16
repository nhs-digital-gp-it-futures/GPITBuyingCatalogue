﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Framework;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Gen2;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.HostingType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType;
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
            Interoperability = new Interoperability(driver, commonActions);
            Implementation = new Implementation(driver, commonActions);
            SolutionApplicationTypes = new SolutionApplicationTypes(driver, commonActions);
            BrowserBased = new BrowserBased(driver, commonActions);
            MobileOrTablet = new MobileOrTablet(driver, commonActions);
            Desktop = new Desktop(driver, commonActions);
            SolutionHostingTypes = new SolutionHostingTypes(driver, commonActions);
            PublicCloud = new PublicCloud(driver, commonActions);
            PrivateCloud = new PrivateCloud(driver, commonActions);
            Hybrid = new Hybrid(driver, commonActions);
            OnPremise = new OnPremise(driver, commonActions);
            ListPrice = new ListPrice(driver, commonActions);
            FlatPrice = new FlatPrice(driver, commonActions);
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

        internal MobileOrTablet MobileOrTablet { get; }

        internal Desktop Desktop { get; }

        internal SolutionHostingTypes SolutionHostingTypes { get; }

        internal PublicCloud PublicCloud { get; }

        internal PrivateCloud PrivateCloud { get; }

        internal Hybrid Hybrid { get; }

        internal OnPremise OnPremise { get; }

        internal ListPrice ListPrice { get; }

        internal FlatPrice FlatPrice { get; }

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
            SolutionApplicationTypes.ApplicationTypeDashboard();
            BrowserBased.AddBrowserBasedApplication();
            BrowserBased.AddBrowserBasedApplicationTypes();
            SolutionApplicationTypes.ApplicationTypeDashboard();
            MobileOrTablet.AddMobileOrTabletApplication();
            MobileOrTablet.AddMobileOrTabletApplicationTypes();
            SolutionApplicationTypes.ApplicationTypeDashboard();
            Desktop.AddDesktopApplication();
            Desktop.AdDesktopApplicationTypes();
            SolutionApplicationTypes.ManageCatalogueSolution();
        }

        public void AddSolutionHostingTypes()
        {
            var solutionId = GetSolutionID();
            SolutionHostingTypes.AddHostingType(solutionId);
            SolutionHostingTypes.HostingTypeDashboard();
            PublicCloud.AddHostingTypePublicCloud();
            SolutionHostingTypes.HostingTypeDashboard();
            PrivateCloud.AddHostingTypePrivateCloud();
            SolutionHostingTypes.HostingTypeDashboard();
            Hybrid.AddHostingTypeHybrid();
            SolutionHostingTypes.HostingTypeDashboard();
            OnPremise.AddHostingTypeOnPremise();
            SolutionHostingTypes.CatalogueSolutionDashboard();
        }

        public void AddSolutionListPrice(ListPriceTypes listPriceTypes)
        {
            var solutionId = GetSolutionID();
            ListPrice.AddListPrice(solutionId);
            if (listPriceTypes == ListPriceTypes.Flat_price)
            {
                FlatPrice.AddFlatPrice(listPriceTypes.ToString());
            }

            ListPrice.ManageSolutions();
        }

        private string GetSolutionID()
        {
            using var dbContext = Factory.DbContext;

            var solution = Driver.Url.Split('/').Last();
            return solution;
        }
    }
}
