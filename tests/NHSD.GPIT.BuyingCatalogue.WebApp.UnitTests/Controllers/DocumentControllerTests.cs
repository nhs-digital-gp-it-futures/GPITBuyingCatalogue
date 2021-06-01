using System;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class DocumentControllerTests
    {
        [Test]
        public static void Constructor_NullDocumentService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DocumentController(null));
        }
    }
}
