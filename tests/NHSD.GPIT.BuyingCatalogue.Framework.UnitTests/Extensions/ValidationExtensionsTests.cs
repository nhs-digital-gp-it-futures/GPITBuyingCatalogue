using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class ValidationExtensionsTests
    {
        [Fact]
        public static void NullObject_ThrowsException()
        {
            const string val = null;

            var actual = Assert.Throws<ArgumentNullException>(() => val.ValidateNotNull("arg"));

            actual.ParamName.Should().Be("arg");
        }

        [Fact]
        public static void ValidObject_DoesNotThrowException()
        {
            const string val = "123";

            var ex = Record.Exception(() => val.ValidateNotNull("arg"));

            ex.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void InvalidString_ThrowsException(string value)
        {
            var actual = Assert.Throws<ArgumentException>(() => value.ValidateNotNullOrWhiteSpace("arg"));

            actual.ParamName.Should().Be("arg");
        }

        [Fact]
        public static void ValidString_DoesNotThrowException()
        {
            const string val = "123";

            var ex = Record.Exception(() => val.ValidateNotNullOrWhiteSpace("arg"));

            ex.Should().BeNull();
        }

        [Fact]
        public static void InvalidGuid_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentException>(() => Guid.Empty.ValidateGuid("id"));

            actual.ParamName.Should().Be("id");
        }

        [Fact]
        public static void ValidGuid_DoesNotThrowException()
        {
            var val = Guid.NewGuid();

            var ex = Record.Exception(() => val.ValidateGuid("arg"));

            ex.Should().BeNull();
        }
    }
}
