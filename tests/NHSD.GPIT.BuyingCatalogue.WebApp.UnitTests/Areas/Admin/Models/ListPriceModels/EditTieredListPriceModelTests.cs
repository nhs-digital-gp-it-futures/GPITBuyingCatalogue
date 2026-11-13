using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels;

public static class EditTieredListPriceModelTests
{
    [Fact]
    public static void NewProperties_CanBeAssigned()
    {
        var solutionId = new CatalogueItemId(1, "1");
        var additionalServiceId = new CatalogueItemId(1, "2");
        var model = new EditTieredListPriceModel
        {
            SolutionType = SolutionType.CommunityPharmacy,
            SolutionId = solutionId,
            AdditionalServiceId = additionalServiceId,
        };

        model.SolutionType.Should().Be(SolutionType.CommunityPharmacy);
        model.SolutionId.Should().Be(solutionId);
        model.AdditionalServiceId.Should().Be(additionalServiceId);
    }
}
