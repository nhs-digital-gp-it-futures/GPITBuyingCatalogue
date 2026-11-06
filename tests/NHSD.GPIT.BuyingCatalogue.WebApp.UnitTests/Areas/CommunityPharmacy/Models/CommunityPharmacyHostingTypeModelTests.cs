using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyHostingTypeModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_MapsHostingTypes(
        Solution solution,
        Hosting hosting,
        CatalogueItemContentStatus contentStatus)
    {
        solution.Hosting = hosting;

        var model = new CommunityPharmacyHostingTypeModel(solution.CatalogueItem, contentStatus);

        model.HybridHostingType.Should().BeSameAs(hosting.HybridHostingType);
        model.OnPremise.Should().BeSameAs(hosting.OnPremise);
        model.PrivateCloud.Should().BeSameAs(hosting.PrivateCloud);
        model.PublicCloud.Should().BeSameAs(hosting.PublicCloud);
        model.SelectedSection.Should().Be("Hosting type");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NullHosting_ReturnsEmptyHostingTypes(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.Hosting = null;

        var model = new CommunityPharmacyHostingTypeModel(solution.CatalogueItem, contentStatus);

        var hosting = new Hosting();
        model.HybridHostingType.Should().BeEquivalentTo(hosting.HybridHostingType);
        model.OnPremise.Should().BeEquivalentTo(hosting.OnPremise);
        model.PrivateCloud.Should().BeEquivalentTo(hosting.PrivateCloud);
        model.PublicCloud.Should().BeEquivalentTo(hosting.PublicCloud);
    }
}
