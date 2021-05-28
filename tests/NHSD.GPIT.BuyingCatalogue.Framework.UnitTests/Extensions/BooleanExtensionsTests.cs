using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class BooleanExtensionsTests
    {
        [TestCase(null, "INCOMPLETE")]
        [TestCase(true, "COMPLETE")]
        [TestCase(false, "INCOMPLETE")]
        public static void BooleanExtension_ToStatus_ReturnsCorrectValue(bool? value, string expected)
        {
            var result = value.ToStatus();
            Assert.AreEqual(result, expected);
        }

        [TestCase(null, "")]
        [TestCase(true, "Yes")]
        [TestCase(false, "No")]
        public static void BooleanExtension_ToYesNo_ReturnsYesOrNoCorrectly(bool? value, string expected)
        {
            var result = value.ToYesNo();
            Assert.AreEqual(result, expected);
        }
    }
}
