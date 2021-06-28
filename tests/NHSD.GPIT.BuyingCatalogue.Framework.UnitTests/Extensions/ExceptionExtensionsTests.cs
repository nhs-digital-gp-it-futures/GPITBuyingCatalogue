using System;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class ExceptionExtensionsTests
    {
        [Fact]
        public static void ExceptionExtension_SimpleException_ReturnsCorrectFullErrorMessage()
        {
            var exception = new Exception("Simple exception");

            Assert.Equal("Simple exception", exception.FullErrorMessage());
        }

        [Fact]
        public static void ExceptionExtension_NestedException_ReturnsCorrectFullErrorMessage()
        {
            var exception = new Exception("Simple exception", new Exception("Inner exception"));

            Assert.Equal("Simple exception. Inner Exception Message: Inner exception", exception.FullErrorMessage());
        }
    }
}
