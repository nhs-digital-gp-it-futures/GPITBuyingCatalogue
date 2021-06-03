using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class TypeExtensionsTests
    {
        [Test]
        public static void ControllerName_TrimsController()
        {
            var result = typeof(TestController).ControllerName();

            Assert.AreEqual("Test", result);
        }

        internal class TestController
        {
        }
    }
}
