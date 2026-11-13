using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels;

public static class TieredPriceTiersModelTests
{
    [Fact]
    public static void NewProperties_CanBeAssigned()
    {
        var additionalServiceId = new CatalogueItemId(1, "2");
        var model = new TieredPriceTiersModel
        {
            SolutionType = SolutionType.CommunityPharmacy,
            AdditionalServiceId = additionalServiceId,
        };

        model.SolutionType.Should().Be(SolutionType.CommunityPharmacy);
        model.AdditionalServiceId.Should().Be(additionalServiceId);
    }
}
