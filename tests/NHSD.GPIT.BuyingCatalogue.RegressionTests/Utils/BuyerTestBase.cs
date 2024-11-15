using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public abstract class BuyerTestBase(
        LocalWebApplicationFactory factory,
        ITestOutputHelper? testOutputHelper = null)
        : TestBase(
            factory,
            testOutputHelper,
            UrlGenerator.GenerateUrlFromMethod(
                typeof(BuyerDashboardController),
                nameof(BuyerDashboardController.Index),
                Parameters))
    {
        private const string InternalOrgId = "IB-QWO";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };
    }
}
