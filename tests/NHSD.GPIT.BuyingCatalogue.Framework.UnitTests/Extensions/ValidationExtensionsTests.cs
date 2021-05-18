using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ValidationExtensionsTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void NullObject_ThrowsException()
        {
            var val = (string)null;
            var actual = Assert.Throws<ArgumentNullException>(() => val.ValidateNotNull("arg"));
            actual.ParamName.Should().Be("arg");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void InvalidString_ThrowsException(string value)
        {
            var actual = Assert.Throws<ArgumentException>(() => value.ValidateNotNullOrWhiteSpace("arg"));
            actual.ParamName.Should().Be("arg");
        }

        [Test]
        public static void InvalidGuid_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentException>(() => Guid.Empty.ValidateGuid("id"));
            actual.ParamName.Should().Be("id");
        }
    }
}
