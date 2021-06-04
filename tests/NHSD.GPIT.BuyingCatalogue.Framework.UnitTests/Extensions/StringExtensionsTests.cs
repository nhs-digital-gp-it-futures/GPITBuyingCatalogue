using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class StringExtensionsTests
    {
        [TestCase("womBAT")]
        [TestCase("123")]
        public static void EqualsIgnoreCase_ToCompareNotSameAsInput_ReturnsFalse(string toCompare)
        {
            "wicked".EqualsIgnoreCase(toCompare).Should().BeFalse();
        }

        [TestCase("WICKED")]
        [TestCase("wicked")]
        [TestCase("WicKeD")]
        public static void EqualsIgnoreCase_ToCompareSameAsInput_ReturnsTrue(string toCompare)
        {
            "wicked".EqualsIgnoreCase(toCompare).Should().BeTrue();
        }
    }
}
