using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionCapabilitiesModelTests
    {
        [Fact]
        public static void Constructor_NullSolutionCapability_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new CatalogueItemCapabilitiesModel(null, new CatalogueItem()));
        }

        [Fact]
        public static void Constructor_NullSolution_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new CatalogueItemCapabilitiesModel(new CatalogueItemCapability(), null));
        }
    }
}
