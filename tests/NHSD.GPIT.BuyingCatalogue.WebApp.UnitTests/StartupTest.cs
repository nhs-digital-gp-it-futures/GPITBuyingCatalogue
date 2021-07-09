using Microsoft.Extensions.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    public sealed class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration)
            : base(configuration)
        {
        }
    }
}
