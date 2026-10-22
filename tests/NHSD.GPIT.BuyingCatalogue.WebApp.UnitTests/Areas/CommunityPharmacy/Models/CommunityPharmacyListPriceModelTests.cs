using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyListPriceModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_OnlyIncludesPublishedFlatPrices(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var prices = Enum.GetValues<CataloguePriceType>()
            .SelectMany(type => Enum.GetValues<PublicationStatus>()
                .Select(status => new CataloguePrice { CataloguePriceType = type, PublishedStatus = status }))
            .ToList();
        solution.CatalogueItem.CataloguePrices = prices;

        var model = new CommunityPharmacyListPriceModel(solution.CatalogueItem, contentStatus);

        model.ListPrices.Should().ContainSingle()
            .Which.Should().BeSameAs(prices.Single(x => x.CataloguePriceType == CataloguePriceType.Flat
                && x.PublishedStatus == PublicationStatus.Published));
        model.SelectedSection.Should().Be("List price");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NoPrices_ReturnsEmptyList(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.CataloguePrices.Clear();

        var model = new CommunityPharmacyListPriceModel(solution.CatalogueItem, contentStatus);

        model.ListPrices.Should().BeEmpty();
    }
}
