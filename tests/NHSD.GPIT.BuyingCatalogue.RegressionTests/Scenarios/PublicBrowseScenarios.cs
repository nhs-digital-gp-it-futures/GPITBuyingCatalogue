using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class PublicBrowseScenarios : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public PublicBrowseScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
           : base(factory, typeof(HomeController), nameof(HomeController.ContactUs), null, testOutputHelper)
        {
        }
    }
}
