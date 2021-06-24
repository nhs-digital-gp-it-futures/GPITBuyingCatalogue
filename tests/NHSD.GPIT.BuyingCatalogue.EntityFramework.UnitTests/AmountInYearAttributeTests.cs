using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AmountInYearAttributeTests
    {
        [Test]
        [AutoData]
        public static void Constructor_Initializes_AmountInYear(
            [Frozen] int expectedAmountInYear,
            AmountInYearAttribute attribute)
        {
            attribute.AmountInYear.Should().Be(expectedAmountInYear);
        }
    }
}
