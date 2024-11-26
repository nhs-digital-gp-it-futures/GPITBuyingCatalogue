using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public abstract class BuyerTestBase : TestBase
    {
        private const string InternalOrgId = "IB-QWO";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        protected BuyerTestBase(
            LocalWebApplicationFactory factory,
            ITestOutputHelper? testOutputHelper = null)
            : base(
                factory,
                testOutputHelper,
                UrlGenerator.GenerateUrlFromMethod(
                    typeof(BuyerDashboardController),
                    nameof(BuyerDashboardController.Index),
                    Parameters))
        {
            BuyerLogin();
        }
    }
}
