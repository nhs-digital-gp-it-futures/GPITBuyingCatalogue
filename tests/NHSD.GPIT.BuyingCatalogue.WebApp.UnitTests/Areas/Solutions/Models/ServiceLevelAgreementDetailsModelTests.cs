using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ServiceLevelAgreementDetailsModelTests
    {
        [Fact]
        public static void Constructing_NullSolution()
            => Assert.Throws<ArgumentNullException>(() => new ServiceLevelAgreementDetailsModel(
                null,
                new CatalogueItemContentStatus()));
    }
}
