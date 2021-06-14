using System;
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

        [TestCase(1, "one")]
        [TestCase(2, "two")]
        [TestCase(3, "three")]
        [TestCase(4, "four")]
        [TestCase(5, "five")]
        [TestCase(6, "six")]
        [TestCase(7, "seven")]
        [TestCase(8, "eight")]
        [TestCase(9, "nine")]
        [TestCase(10, "ten")]
        public static void ToEnglish_ValidInputs_ResultAsExpected(int input, string expected)
        {
            input.ToEnglish().Should().Be(expected);
        }

        [TestCase(0)]
        [TestCase(-4)]
        [TestCase(11)]
        [TestCase(42)]
        public static void ToEnglish_InvalidInputs_ThrowsException(int invalid)
        {
            Assert.Throws<ArgumentException>(() => invalid.ToEnglish())
                .Message.Should()
                .Be($"This method is valid only for numbers from 1 to 10");
        }
    }
}
