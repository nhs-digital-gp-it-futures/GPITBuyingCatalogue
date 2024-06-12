using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class ByteArrayExtensionTests
    {
        [Fact]
        public static void GetString_NullArray_ReturnsEmptyString()
        {
            byte[] val = null;

            var result = val.GetString();

            result.Should().BeEmpty();
        }

        [Fact]
        public static void GetString_EmptyArray_ReturnsEmptyString()
        {
            byte[] val = System.Array.Empty<byte>();

            var result = val.GetString();

            result.Should().BeEmpty();
        }

        [Theory]
        [MockAutoData]
        public static void GetString_ValidArray_ReturnsCorrectString(string expected)
        {
            var bytes = Encoding.UTF8.GetBytes(expected);

            var result = bytes.GetString();

            result.Should().BeEquivalentTo(expected);
        }
    }
}
