using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class AdminScenarios : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdminScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
           : base(factory, typeof(HomeController), nameof(HomeController.Index), null, testOutputHelper)
        {
        }

        [Fact]
        [Trait("Admin", "Gen2")]
        public void Gen2CapabilitiesAndEpicsMapping()
        {
            AdminPages.AdminDashboard.ManageCapabilitiesAndEpics();
        }
    }
}
