using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class TypeExtensionsTests
    {
        [Fact]
        public static void ControllerName_TrimsController()
        {
            var result = typeof(TestController).ControllerName();

            Assert.Equal("Test", result);
        }

        internal static class TestController
        {
        }
    }
}
