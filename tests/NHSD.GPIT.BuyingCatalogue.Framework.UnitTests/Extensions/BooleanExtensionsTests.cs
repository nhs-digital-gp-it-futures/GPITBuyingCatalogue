using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class BooleanExtensionsTests
    {
        [Test]
        [TestCase(null, "No")]
        [TestCase("true", "Yes")]
        [TestCase("false", "No")]
        public static void BooleanExtension_TpYesNo_ReturnsYesOrNoCorrectly(bool? value, string expected)
        {
            var result = value.ToYesNo();
            Assert.AreEqual(result, expected);
        }      
    }
}
