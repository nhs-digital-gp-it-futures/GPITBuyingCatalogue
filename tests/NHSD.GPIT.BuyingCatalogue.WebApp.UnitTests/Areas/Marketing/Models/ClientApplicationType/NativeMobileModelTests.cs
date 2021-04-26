using System;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeMobileModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileModel(null));
        }
    }
}
