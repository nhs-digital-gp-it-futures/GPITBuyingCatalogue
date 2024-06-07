using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class AdminScenarios : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string CapabilitiesFileName = "CapabilitiesAndSolutions.csv";
        private const string EpicsFileName = "EpicsAndSolutions.csv";
        private const string FailedEpicsFileName = "EpicsAndSolutions_MissingData.csv";
        private const string FailedCapabilitiesFileName = "CapabilitiesAndSolutions_MissingData.csv";
        private const string OrganisationName = "NHS HUMBER AND NORTH YORKSHIRE INTEGRATED CARE BOARD";
        private const string AdminOrganisationName = "NHS Digital";

        public AdminScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
           : base(factory, typeof(HomeController), nameof(HomeController.Index), null, testOutputHelper)
        {
        }

        [Fact]
        [Trait("Gen2", "CapabilityAndEpics")]
        public void Gen2CapabilitiesAndEpicsMappingSuccess()
        {
            AdminPages.AdminDashboard.ManageCapabilitiesAndEpics();

            AdminPages.CapabilitiesAndEpicsMappings.ImportCapabilities(CapabilitiesFileName);

            AdminPages.CapabilitiesAndEpicsMappings.ImportEpics(EpicsFileName);

            AdminPages.CapabilitiesAndEpicsMappings.CapabilitiesAndEpicsMappingSuccess();
        }

        [Fact]
        [Trait("Gen2", "CapabilityAndEpics")]
        public void Gen2FailedCapabilities()
        {
            AdminPages.AdminDashboard.ManageCapabilitiesAndEpics();

            AdminPages.CapabilitiesAndEpicsMappings.ImportCapabilities(FailedCapabilitiesFileName);

            AdminPages.CapabilitiesAndEpicsMappings.CapabilitiesAndEpicsMappinFailed();
        }

        [Fact]
        [Trait("Gen2", "CapabilityAndEpics")]
        public void Gen2FailedEpics()
        {
            AdminPages.AdminDashboard.ManageCapabilitiesAndEpics();

            AdminPages.CapabilitiesAndEpicsMappings.ImportCapabilities(CapabilitiesFileName);

            AdminPages.CapabilitiesAndEpicsMappings.ImportEpics(FailedEpicsFileName);

            AdminPages.CapabilitiesAndEpicsMappings.SolutionsAndEpicsMappinFailed();
        }

        [Fact]
        [Trait("Framework", "AddNewFramework")]
        public void AddNewFramework()
        {
            AdminPages.AdminDashboard.ManageFrameworks();

            AdminPages.AddFramework.NewFramework();

            AdminPages.AddFramework.AddFrameworkDetails();
        }

        [Fact]
        [Trait("Solutions", "AddNewSolutionsForProviderAndFlatPrice")]
        public void AddNewSolutionsForProviderAndFlatPrice()
        {
            AdminPages.AdminDashboard.ManageCatalogueSolutions();

            AdminPages.AddSolutionDetailsAndDescription();

            AdminPages.AddSolutionInteroperability(ProviderOrConsumer.Provider);

            AdminPages.AddSolutionImplementation();

            AdminPages.AddSolutionApplicationTypes();

            AdminPages.AddSolutionHostingTypes();

            AdminPages.AddSolutionListPrice(ListPriceTypes.Flat_price);

            AdminPages.AddSolutionCapabilitiesAndEpics();

            AdminPages.AddAdditionalService(ListPriceTypes.Flat_price);

            AdminPages.AddAssociatedService(ListPriceTypes.Flat_price);

            AdminPages.AddWorkOffPlans();

            AdminPages.AddSupplierDetails();
        }

        [Fact]
        [Trait("Solutions", "AddNewSolutionsForProviderAndTieredPrice")]
        public void AddNewSolutionsForProviderAndTieredPrice()
        {
            AdminPages.AdminDashboard.ManageCatalogueSolutions();

            AdminPages.AddSolutionDetailsAndDescription();

            AdminPages.AddSolutionInteroperability(ProviderOrConsumer.Provider);

            AdminPages.AddSolutionImplementation();

            AdminPages.AddSolutionApplicationTypes();

            AdminPages.AddSolutionHostingTypes();

            AdminPages.AddSolutionListPrice(ListPriceTypes.Tiered_price);

            AdminPages.AddSolutionCapabilitiesAndEpics();

            AdminPages.AddAdditionalService(ListPriceTypes.Tiered_price);

            AdminPages.AddAssociatedService(ListPriceTypes.Tiered_price);

            AdminPages.AddWorkOffPlans();

            AdminPages.AddSupplierDetails();
        }

        [Fact]
        [Trait("SupplierDefindEpics", "AddNewSupplierDefindEpics")]
        public void AddNewSupplierDefindEpics()
        {
            AdminPages.AdminDashboard.ManageSupplierDefinedEpics();

            AdminPages.AddSupplierDefinedEpics.AddNewSupplierDefinedEpic();

            AdminPages.AddSupplierDefinedEpics.SupplierDefinedEpicDetails();

            AdminPages.AddSupplierDefinedEpics.SupplierDefinedEpicInformation();
        }

        [Fact]
        [Trait("ManageUsers", "Users")]
        public void AddNewOranisatinBuyerUser()
        {
            AdminPages.AdminDashboard.ManageAllUsers();

            AdminPages.AddOrganisationUser.AddNewUser();

            AdminPages.AddOrganisationUser.NewUserDetails(OrganisationName);
        }

        [Fact]
        [Trait("ManageSupplier", "Suppliers")]
        public void AddNewOranisatinSupplier()
        {
            AdminPages.AdminDashboard.ManageSupplier();

            AdminPages.ManageOrganisationSupplier.AddSupplierDetails();

            AdminPages.ManageOrganisationSupplier.AddSupplilerAddress();

            AdminPages.ManageOrganisationSupplier.AddSupplierContactDetails();

            AdminPages.ManageOrganisationSupplier.AddSupplierStatus();
        }
    }
}
