using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ClientApplicationTypeModelTests
    {
        [Fact]
        public static void Constructor_NullClient_SetsDisplayFalse()
        {
            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.BrowserBased, null);

            Assert.False(model.DisplayClientApplication);
        }
    }
}
