using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Shortlist;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class ShortlistScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : BuyerTestBase(
            factory,
            typeof(BuyerDashboardController),
            nameof(BuyerDashboardController.Index),
            Parameters,
            testOutputHelper), IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "IB-QWO";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        [Fact]
        [Trait("Shortlist", "Create shortlist")]
        public void CreateNewShortlistForFoundationCapabilities()
        {
            string shortlistName = "Filter by Foundation Capabilities";

            ShortlistPages.CreateNewShortlist();

            ShortlistPages.CreateShortlistForFoundationCapabilities(shortlistName);
        }

        [Fact]
        [Trait("Shortlist", "Create shortlist")]
        public void CreateNewShortlistForFramework()
        {
            string shortlistName = "Filter by Framework";

            ShortlistPages.CreateNewShortlist();

            ShortlistPages.CreateShortlistForFramework(shortlistName);
        }

        [Fact]
        [Trait("Shortlist", "Create shortlist")]
        public void CreateNewShortlistForApplicationTypes()
        {
            string shortlistName = "Filter by Application type";

            ShortlistPages.CreateNewShortlist();

            ShortlistPages.CreateShortlistForApplicationType(shortlistName, ApplicationTypes.Desktop);
        }

        [Fact]
        [Trait("Shortlist", "Create shortlist")]
        public void CreateNewShortlistForHostingTypes()
        {
            string shortlistName = "Filter by Hosting type";

            ShortlistPages.CreateNewShortlist();

            ShortlistPages.CreateShortlistForHostingType(shortlistName, HostingTypes.Public_cloud);
        }
    }
}
