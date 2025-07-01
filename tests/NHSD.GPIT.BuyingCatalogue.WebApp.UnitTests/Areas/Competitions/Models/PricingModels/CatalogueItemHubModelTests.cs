using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.PricingModels;

public static class CatalogueItemHubModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice)
    {
        selectedPrice.Tiers = null;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.SolutionId.Should().Be(solutionId);
        model.CatalogueItemName.Should().Be(catalogueItem.Name);
        model.CatalogueItemId.Should().Be(catalogueItem.Id);
        model.CatalogueItemType.Should().Be(catalogueItem.CatalogueItemType);
        model.NumberOfCataloguePrices.Should().Be(catalogueItem.CataloguePrices.Count);
        model.OdsOrganisations.Should().BeEquivalentTo(recipientQuantities);
        model.Price.Should().Be(selectedPrice);
        model.GlobalQuantity.Should().Be(globalQuantity);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithPrice_SetsPriceIdAsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceId.Should().Be(selectedPrice.CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithInvalidPrice_SetsPriceIdAsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CataloguePrice> catalogueItemPrices)
    {
        selectedPrice.Tiers = null;
        catalogueItem.CataloguePrices = catalogueItemPrices;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceId.Should().Be(catalogueItem.CataloguePrices.First().CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static void PriceProgress_NoTiers_Expected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CataloguePrice> catalogueItemPrices)
    {
        selectedPrice.Tiers = null;
        catalogueItem.CataloguePrices = catalogueItemPrices;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void PriceProgress_WithTiers_Expected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_PriceNotCompleted_AsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CataloguePrice> catalogueItemPrices)
    {
        selectedPrice.Tiers = null;
        catalogueItem.CataloguePrices = catalogueItemPrices;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.NotStarted);
        model.QuantityProgress.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_GlobalQuantityDefined_AsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            globalQuantity,
            null,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
        model.QuantityProgress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_RecipientQuantitiesDefined_AsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        List<CompetitionSublocationRecipient> recipients,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            null,
            recipients.ToDictionary(x => x, x => (int?)5),
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
        model.QuantityProgress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_RecipientQuantityMissing_AsExpected(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        List<CompetitionSublocationRecipient> recipients,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        CompetitionSublocationRecipient organisation = recipients.First();

        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities =
            recipients.ToDictionary(x => x, x => (int?)5);

        recipientQuantities[organisation] = null;

        var model = new CatalogueItemHubModel(
            solutionId,
            catalogueItem,
            null,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
        model.QuantityProgress.Should().Be(TaskProgress.InProgress);
    }
}
