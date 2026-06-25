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

public static class AdditionalServiceAssociatedServiceItemModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice)
    {
        selectedPrice.Tiers = null;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.AdditionalServiceItemId.Should().Be(additionalServiceItemId);
        model.AssociatedServiceItem.Should().Be(associatedServiceItem);
        model.NumberOfCataloguePrices.Should().Be(associatedServiceItem.CataloguePrices.Count);
        model.OdsOrganisations.Should().BeEquivalentTo(recipientQuantities);
        model.Price.Should().Be(selectedPrice);
        model.GlobalQuantity.Should().Be(globalQuantity);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithPrice_SetsPriceIdAsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceId.Should().Be(selectedPrice.CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithInvalidPrice_SetsPriceIdAsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CataloguePrice> catalogueItemPrices)
    {
        selectedPrice.Tiers = null;
        associatedServiceItem.CataloguePrices = catalogueItemPrices;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceId.Should().Be(associatedServiceItem.CataloguePrices.First().CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static void PriceProgress_NoTiers_Expected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CataloguePrice> catalogueItemPrices)
    {
        selectedPrice.Tiers = null;
        associatedServiceItem.CataloguePrices = catalogueItemPrices;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void PriceProgress_WithTiers_Expected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_PriceNotCompleted_AsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CataloguePrice> catalogueItemPrices)
    {
        selectedPrice.Tiers = null;
        associatedServiceItem.CataloguePrices = catalogueItemPrices;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.NotStarted);
        model.QuantityProgress.Should().Be(TaskProgress.CannotStart);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_GlobalQuantityDefined_AsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        int? globalQuantity,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            globalQuantity,
            null,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
        model.QuantityProgress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_RecipientQuantitiesDefined_AsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        List<CompetitionSublocationRecipient> recipients,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            null,
            recipients.ToDictionary(x => x, x => (int?)5),
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
        model.QuantityProgress.Should().Be(TaskProgress.Completed);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_RecipientQuantityMissing_AsExpected(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        List<CompetitionSublocationRecipient> recipients,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;

        CompetitionSublocationRecipient organisation = recipients.First();

        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities =
            recipients.ToDictionary(x => x, x => (int?)5);

        recipientQuantities[organisation] = null;

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            null,
            recipientQuantities,
            selectedPrice);

        model.PriceProgress.Should().Be(TaskProgress.Completed);
        model.QuantityProgress.Should().Be(TaskProgress.InProgress);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_RecipientQuantitiesDefined_AsNotStarted(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        CompetitionCatalogueItemPrice selectedPrice,
        CompetitionSublocationRecipient competitionSublocationRecipient,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities = new()
        {
            { competitionSublocationRecipient, null },
        };

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            null,
            recipientQuantities,
            selectedPrice);

        model.QuantityProgress.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void QuantityProgress_RecipientQuantitiesDefined_AsInProgress(
        CatalogueItemId additionalServiceItemId,
        CatalogueItem associatedServiceItem,
        CompetitionCatalogueItemPrice selectedPrice,
        List<CompetitionCatalogueItemPriceTier> tiers)
    {
        selectedPrice.Tiers = tiers;
        Dictionary<CompetitionSublocationRecipient, int?> recipientQuantities = new()
        {
            { new CompetitionSublocationRecipient() { CompetitionId = 1 }, 100 },
            { new CompetitionSublocationRecipient() { CompetitionId = 2 }, 200 },
            { new CompetitionSublocationRecipient() { CompetitionId = 3 }, null },
        };

        var model = new AdditionalServiceAssociatedServiceItemModel(
            additionalServiceItemId,
            associatedServiceItem,
            null,
            recipientQuantities,
            selectedPrice);

        model.QuantityProgress.Should().Be(TaskProgress.InProgress);
    }
}
