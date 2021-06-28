using System;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class DocumentControllerTests
    {
        [Fact]
        public static void Constructor_NullDocumentService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DocumentController(null));
        }
    }
}
