using System;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ExceptionExtensionsTests
    {
        [Test]

        public static void ExceptionExtension_SimpleException_ReturnsCorrectFullErrorMessage()
        {
            var exception = new Exception("Simple exception");

            Assert.AreEqual("Simple exception", exception.FullErrorMessage());
        }

        [Test]
        public static void ExceptionExtension_NestedException_ReturnsCorrectFullErrorMessage()
        {
            var exception = new Exception("Simple exception", new Exception("Inner exception"));

            Assert.AreEqual("Simple exception. Inner Exception Message: Inner exception", exception.FullErrorMessage());
        }
    }
}
