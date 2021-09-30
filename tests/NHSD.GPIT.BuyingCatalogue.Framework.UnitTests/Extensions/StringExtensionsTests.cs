using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class StringExtensionsTests
    {
        [Theory]
        [InlineData("womBAT")]
        [InlineData("123")]
        public static void EqualsIgnoreCase_ToCompareNotSameAsInput_ReturnsFalse(string toCompare)
        {
            "wicked".EqualsIgnoreCase(toCompare).Should().BeFalse();
        }

        [Theory]
        [InlineData("WICKED")]
        [InlineData("wicked")]
        [InlineData("WicKeD")]
        public static void EqualsIgnoreCase_ToCompareSameAsInput_ReturnsTrue(string toCompare)
        {
            "wicked".EqualsIgnoreCase(toCompare).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, "one")]
        [InlineData(2, "two")]
        [InlineData(3, "three")]
        [InlineData(4, "four")]
        [InlineData(5, "five")]
        [InlineData(6, "six")]
        [InlineData(7, "seven")]
        [InlineData(8, "eight")]
        [InlineData(9, "nine")]
        [InlineData(10, "ten")]
        public static void ToEnglish_ValidInputs_ResultAsExpected(int input, string expected)
        {
            input.ToEnglish().Should().Be(expected);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-4)]
        [InlineData(11)]
        [InlineData(42)]
        public static void ToEnglish_InvalidInputs_ThrowsException(int invalid)
        {
            Assert.Throws<ArgumentException>(() => invalid.ToEnglish())
                .Message.Should()
                .Be("This method is valid only for numbers from 1 to 10");
        }

        [Theory]
        [InlineData("abc", "abc")]
        [InlineData(" abc", "abc")]
        [InlineData("abc ", "abc")]
        [InlineData("a b c", "abc")]
        public static void FormatForComparison_ValidInputs_ResultAsExpected(string input, string expected)
        {
            input.FormatForComparison().Should().Be(expected);
        }

        [Theory]
        [InlineData("abc", "abc", true)]
        [InlineData(" abc", "abc", true)]
        [InlineData("abc ", "abc", true)]
        [InlineData("a b c", "abc", true)]
        [InlineData("abc", "abc ", true)]
        [InlineData("abc", " abc", true)]
        [InlineData("abc", "a b c", true)]
        [InlineData("abc", "A B C", true)]
        [InlineData("abc", "abcd", false)]
        public static void EqualsIgnoreWhiteSpace_ValidInputs_ResultAsExpected(string input1, string input2, bool expected)
        {
            var actual = input1.EqualsIgnoreWhiteSpace(input2);
            actual.Should().Be(expected);
        }
    }
}
