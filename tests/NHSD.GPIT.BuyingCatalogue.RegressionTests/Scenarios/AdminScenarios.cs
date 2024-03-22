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

        public AdminScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
           : base(factory, typeof(HomeController), nameof(HomeController.Index), null, testOutputHelper)
        {
        }

        [Fact]
        [Trait("Gen2", "Success")]
        public void Gen2CapabilitiesAndEpicsMappingSuccess()
        {
            AdminPages.AdminDashboard.ManageCapabilitiesAndEpics();

            AdminPages.CapabilitiesAndEpicsMappings.ImportCapabilities(CapabilitiesFileName);

            AdminPages.CapabilitiesAndEpicsMappings.ImportEpics(EpicsFileName);

            AdminPages.CapabilitiesAndEpicsMappings.CapabilitiesAndEpicsMappingSuccess();
        }

        [Fact]
        [Trait("Framework", "AddNewFramework")]
        public void AddNewFramework()
        {
            AdminPages.AdminDashboard.ManageFrameworks();

            AdminPages.AddFramework.NewFramework();

            AdminPages.AddFramework.AddFrameworkDetails();
        }
    }
}
