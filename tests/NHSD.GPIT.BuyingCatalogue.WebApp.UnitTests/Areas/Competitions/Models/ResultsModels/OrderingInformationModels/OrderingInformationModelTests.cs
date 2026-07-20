using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;

public static class OrderingInformationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int expectedRecipientCount,
        Solution solution,
        CompetitionSolution competitionSolution)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;
        competitionSolution.CatalogueItem = solution.CatalogueItem;

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);

        model.NumberOfRecipients.Should().Be(expectedRecipientCount);
        model.ContractLength.Should().Be(competition.ContractLength);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SolutionGlobalQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int expectedRecipientCount,
        Solution solution,
        CompetitionSolution competitionSolution,
        int globalQuantity)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Quantity = globalQuantity;

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        model.SolutionDisplay.Should()
            .BeEquivalentTo(
                new OrderingInformationItem(solution.CatalogueItem, competitionSolution.Price, globalQuantity));
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SolutionRecipientQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int expectedRecipientCount,
        Solution solution,
        CompetitionSolution competitionSolution,
        int recipientQuantity)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Quantity = null;
        competitionSolution.Quantities = competition.FlattenedRecipients
            .Select(x =>
                new CompetitionItemQuantity()
                {
                    CompetitionId = competition.Id,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = recipientQuantity,
                })
            .ToList();

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        model.SolutionDisplay.Should()
            .BeEquivalentTo(
                new OrderingInformationItem(solution.CatalogueItem, competitionSolution.Price, competitionSolution.Quantities.Sum(x => x.Quantity)));
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SolutionServiceGlobalQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int expectedRecipientCount,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService solutionService,
        int globalQuantity)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Services = [solutionService];

        solutionService.CatalogueItem = additionalService.CatalogueItem;
        solutionService.Quantity = globalQuantity;

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        var expectedItems = new List<OrderingInformationItem>
        {
            new(
                solutionService.CatalogueItem,
                solutionService.Price,
                globalQuantity),
        };

        model.Items.Should().BeEquivalentTo(expectedItems);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SolutionServiceRecipientQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int expectedRecipientCount,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionAssociatedService competitionAssociatedService,
        int recipientQuantity)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        competitionAssociatedService.CatalogueItemId = associatedService.CatalogueItem.Id;
        competitionAssociatedService.CatalogueItem = associatedService.CatalogueItem;
        competitionAssociatedService.Quantity = null;
        competitionAssociatedService.Quantities = [.. competition.FlattenedRecipients
            .Select(x =>
                new CompetitionItemQuantity()
                {
                    CompetitionId = competition.Id,
                    ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = recipientQuantity,
                })];

        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItem.Id;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.Quantity = null;
        competitionAdditionalService.Services = [competitionAssociatedService];
        competitionAdditionalService.Quantities = [.. competition.FlattenedRecipients
            .Select(x =>
                new CompetitionItemQuantity()
                {
                    CompetitionId = competition.Id,
                    ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = recipientQuantity,
                })];

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Services = [competitionAdditionalService];

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        var expectedServices = new List<OrderingInformationItem>
        {
            new(
                competitionAssociatedService.CatalogueItem,
                competitionAssociatedService.Price,
                competitionAssociatedService.Quantities.Sum(x => x.Quantity)),
        };

        var expectedItems = new List<OrderingInformationItem>
        {
            new(
                competitionAdditionalService.CatalogueItem,
                competitionAdditionalService.Price,
                competitionAdditionalService.Quantities.Sum(x => x.Quantity))
            {
                Services = expectedServices,
            },
        };

        model.Items.Should().BeEquivalentTo(expectedItems);
        model.Items.Count.Should().Be(1);
        model.Items.ElementAt(0).Services.Count().Should().Be(1);
        model.Items.ElementAt(0).Services.Should().BeEquivalentTo(expectedServices);
    }

    [Theory]
    [MockAutoData]
    public static void GetAssociatedServices_ReturnsAssociatedServices(
        OrderingInformationModel model)
    {
        var additionalServiceItem = model.Items.First();
        var associatedServiceItem = model.Items.Skip(1).First();

        additionalServiceItem.CatalogueItemType = CatalogueItemType.AdditionalService;
        associatedServiceItem.CatalogueItemType = CatalogueItemType.AssociatedService;

        var associatedServices = model.GetAssociatedServices();

        associatedServices.Should().BeEquivalentTo(model.Items.Where(x => x.CatalogueItemType == CatalogueItemType.AssociatedService));
    }

    [Theory]
    [MockAutoData]
    public static void GetAdditionalServices_ReturnsAdditionalServices(
        OrderingInformationModel model)
    {
        var additionalServiceItem = model.Items.First();
        var associatedServiceItem = model.Items.Skip(1).First();

        additionalServiceItem.CatalogueItemType = CatalogueItemType.AdditionalService;
        associatedServiceItem.CatalogueItemType = CatalogueItemType.AssociatedService;

        var additionalServices = model.GetAdditionalServices();

        additionalServices.Should().BeEquivalentTo(model.Items.Where(x => x.CatalogueItemType == CatalogueItemType.AdditionalService));
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalOneOffCost_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int recipientCount,
        int recipientQuantity,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionAssociatedService competitionAssociatedService,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPrice additionalServicePrice,
        CompetitionCatalogueItemPrice associatedServicePrice,
        CompetitionCatalogueItemPriceTier tier)
    {
        tier.LowerRange = 0;
        tier.UpperRange = null;

        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        associatedServicePrice.BillingPeriod = null;
        associatedServicePrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        associatedServicePrice.Tiers = [tier];
        competitionAssociatedService.Price = associatedServicePrice;
        competitionAssociatedService.CatalogueItemId = associatedService.CatalogueItem.Id;
        competitionAssociatedService.CatalogueItem = associatedService.CatalogueItem;
        competitionAssociatedService.Quantity = null;
        competitionAssociatedService.Quantities = [.. competition.FlattenedRecipients
            .Select(x =>
                new CompetitionItemQuantity()
                {
                    CompetitionId = competition.Id,
                    ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = recipientQuantity,
                })];

        additionalServicePrice.BillingPeriod = null;
        additionalServicePrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        additionalServicePrice.Tiers = [tier];
        competitionAdditionalService.Price = additionalServicePrice;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItem.Id;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.Services = [competitionAssociatedService];
        competitionAdditionalService.Quantity = null;
        competitionAdditionalService.Quantities = [.. competition.FlattenedRecipients
            .Select(x =>
                new CompetitionItemQuantity()
                {
                    CompetitionId = competition.Id,
                    ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = recipientQuantity,
                })];

        solutionPrice.BillingPeriod = null;
        solutionPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        solutionPrice.Tiers = [tier];
        competitionSolution.CatalogueItemId = solution.CatalogueItem.Id;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Services = [competitionAdditionalService];
        competitionSolution.Price = solutionPrice;

        var solutionTotalQuantity = competitionSolution.Quantities.Sum(x => x.Quantity);
        var expectedTotalCost = ((IPrice)competitionSolution.Price).CalculateOneOffCost(solutionTotalQuantity.Value)
            + ((IPrice)competitionAssociatedService.Price).CalculateOneOffCost(solutionTotalQuantity.Value)
            + ((IPrice)competitionAdditionalService.Price).CalculateOneOffCost(solutionTotalQuantity.Value);

        var model = new OrderingInformationModel(competition, competitionSolution, recipientCount);
        var result = model.CalculateTotalOneOffCost();

        result.Should().Be(expectedTotalCost);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalMonthlyCost_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int recipientCount,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionAssociatedService competitionAssociatedService,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPrice additionalServicePrice,
        CompetitionCatalogueItemPrice associatedServicePrice,
        CompetitionCatalogueItemPriceTier tier)
    {
        tier.LowerRange = 0;
        tier.UpperRange = null;

        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        associatedServicePrice.BillingPeriod = TimeUnit.PerMonth;
        associatedServicePrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        associatedServicePrice.Tiers = [tier];
        competitionAssociatedService.Price = associatedServicePrice;
        competitionAssociatedService.CatalogueItemId = associatedService.CatalogueItem.Id;
        competitionAssociatedService.CatalogueItem = associatedService.CatalogueItem;
        competitionAssociatedService.Quantity = 1;

        additionalServicePrice.BillingPeriod = TimeUnit.PerMonth;
        additionalServicePrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        additionalServicePrice.Tiers = [tier];
        competitionAdditionalService.Price = additionalServicePrice;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItem.Id;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.Services = [competitionAssociatedService];
        competitionAdditionalService.Quantity = 1;

        solutionPrice.BillingPeriod = TimeUnit.PerMonth;
        solutionPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        solutionPrice.Tiers = [tier];
        competitionSolution.CatalogueItemId = solution.CatalogueItem.Id;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Services = [competitionAdditionalService];
        competitionSolution.Price = solutionPrice;

        var solutionTotalQuantity = competitionSolution.Quantities.Sum(x => x.Quantity);
        var expectedTotalCost = ((IPrice)competitionSolution.Price).CalculateCostPerMonth(solutionTotalQuantity.Value)
            + ((IPrice)competitionAssociatedService.Price).CalculateCostPerMonth(solutionTotalQuantity.Value)
            + ((IPrice)competitionAdditionalService.Price).CalculateCostPerMonth(solutionTotalQuantity.Value);

        var model = new OrderingInformationModel(competition, competitionSolution, recipientCount);
        var result = model.CalculateTotalMonthlyCost();

        result.Should().Be(expectedTotalCost);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalYearlyCost_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        int recipientCount,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionAssociatedService competitionAssociatedService,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPrice additionalServicePrice,
        CompetitionCatalogueItemPrice associatedServicePrice,
        CompetitionCatalogueItemPriceTier tier)
    {
        tier.LowerRange = 0;
        tier.UpperRange = null;

        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        associatedServicePrice.BillingPeriod = TimeUnit.PerYear;
        associatedServicePrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        associatedServicePrice.Tiers = [tier];
        competitionAssociatedService.Price = associatedServicePrice;
        competitionAssociatedService.CatalogueItemId = associatedService.CatalogueItem.Id;
        competitionAssociatedService.CatalogueItem = associatedService.CatalogueItem;
        competitionAssociatedService.Quantity = 1;

        additionalServicePrice.BillingPeriod = TimeUnit.PerYear;
        additionalServicePrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        additionalServicePrice.Tiers = [tier];
        competitionAdditionalService.Price = additionalServicePrice;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItem.Id;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.Services = [competitionAssociatedService];
        competitionAdditionalService.Quantity = 1;

        solutionPrice.BillingPeriod = TimeUnit.PerYear;
        solutionPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
        solutionPrice.Tiers = [tier];
        competitionSolution.CatalogueItemId = solution.CatalogueItem.Id;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Services = [competitionAdditionalService];
        competitionSolution.Price = solutionPrice;

        var solutionTotalQuantity = competitionSolution.Quantities.Sum(x => x.Quantity);
        var expectedTotalCost = ((IPrice)competitionSolution.Price).CalculateCostPerYear(solutionTotalQuantity.Value)
            + ((IPrice)competitionAssociatedService.Price).CalculateCostPerYear(solutionTotalQuantity.Value)
            + ((IPrice)competitionAdditionalService.Price).CalculateCostPerYear(solutionTotalQuantity.Value);

        var model = new OrderingInformationModel(competition, competitionSolution, recipientCount);
        var result = model.CalculateTotalYearlyCost();

        result.Should().Be(expectedTotalCost);
    }
}
