using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public abstract class AnonymousTestBase : TestBase
    {
        protected AnonymousTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string>? parameters = null,
            ITestOutputHelper? testOutputHelper = null)
            : base(
                  factory,
                  testOutputHelper,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
        }

        protected AnonymousTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string>? parameters,
            string buyerEmail,
            ITestOutputHelper? testOutputHelper = null)
            : base(
                  factory,
                  testOutputHelper,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
        }
    }
}
