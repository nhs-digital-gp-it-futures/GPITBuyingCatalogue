using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionCapabilitiesModelTests
    {
        [Test]
        public static void Constructor_NullSolutionCapability_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionCapabilitiesModel(null, new Solution()));
        }

        [Test]
        public static void Constructor_NullSolution_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionCapabilitiesModel(new SolutionCapability(), null));
        }
    }
}
