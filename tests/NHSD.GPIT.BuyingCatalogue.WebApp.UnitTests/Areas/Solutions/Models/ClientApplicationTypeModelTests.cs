using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClientApplicationTypeModelTests
    {
        [Test]
        public static void Constructor_NullClient_SetsDisplayFalse()
        {
            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.BrowserBased, null);

            Assert.False(model.DisplayClientApplication);
        }
    }
}
