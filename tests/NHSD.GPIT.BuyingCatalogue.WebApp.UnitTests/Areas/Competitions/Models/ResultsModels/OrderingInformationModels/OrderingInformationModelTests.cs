using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
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
        competitionSolution.Solution = solution;

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
        competitionSolution.Solution = solution;
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
        competitionSolution.Solution = solution;
        competitionSolution.Quantity = null;
        competitionSolution.Quantities = competition.FlattenedRecipients
            .Select(x =>
                new SolutionQuantitySublocationRecipient
                {
                    RecipientOdsCode = x.RecipientOdsCode, Quantity = recipientQuantity,
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
        SolutionService solutionService,
        int globalQuantity)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        solutionService.Service = additionalService.CatalogueItem;
        solutionService.Quantity = globalQuantity;

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        var expectedItems = new List<OrderingInformationItem>
        {
            new(
                solutionService.Service,
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
        SolutionService solutionService,
        int recipientQuantity)
    {
        competition.CompetitionSublocations = competitionSublocations;
        competition.Organisation = organisation;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        solutionService.Service = additionalService.CatalogueItem;
        solutionService.Quantity = null;
        solutionService.Quantities = competition.FlattenedRecipients
            .Select(x =>
                new ServiceQuantitySublocationRecipient
                {
                    ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = recipientQuantity,
                })
            .ToList();

        var model = new OrderingInformationModel(competition, competitionSolution, expectedRecipientCount);

        var expectedItems = new List<OrderingInformationItem>
        {
            new(
                solutionService.Service,
                solutionService.Price,
                solutionService.Quantities.Sum(x => x.Quantity)),
        };

        model.Items.Should().BeEquivalentTo(expectedItems);
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

        associatedServices.Should().BeEquivalentTo(new List<OrderingInformationItem> { associatedServiceItem });
    }

    [Theory]
    [MockAutoData]
    public static void GetAdditionalServices_ReturnsAssociatedServices(
        OrderingInformationModel model)
    {
        var additionalServiceItem = model.Items.First();
        var associatedServiceItem = model.Items.Skip(1).First();

        additionalServiceItem.CatalogueItemType = CatalogueItemType.AdditionalService;
        associatedServiceItem.CatalogueItemType = CatalogueItemType.AssociatedService;

        var additionalServices = model.GetAdditionalServices();

        additionalServices.Should().BeEquivalentTo(new List<OrderingInformationItem> { additionalServiceItem });
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalOneOffCost_ReturnsExpected(
        OrderingInformationModel model,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPriceTier tier)
    {
        solutionPrice.BillingPeriod = null;
        solutionPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;

        tier.LowerRange = 0;
        tier.UpperRange = null;

        solutionPrice.Tiers = new List<CompetitionCatalogueItemPriceTier> { tier };

        model.SolutionDisplay.Price = solutionPrice;

        var result = model.CalculateTotalOneOffCost();

        result.Should().Be(tier.Price);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalMonthlyCost_ReturnsExpected(
        OrderingInformationModel model,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPriceTier tier)
    {
        solutionPrice.BillingPeriod = TimeUnit.PerMonth;
        solutionPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;

        tier.LowerRange = 0;
        tier.UpperRange = null;

        solutionPrice.Tiers = new List<CompetitionCatalogueItemPriceTier> { tier };

        model.SolutionDisplay.Price = solutionPrice;
        model.SolutionDisplay.Quantity = 1;

        var result = model.CalculateTotalMonthlyCost();

        result.Should().Be(tier.Price);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateTotalYearlyCost_ReturnsExpected(
        OrderingInformationModel model,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPriceTier tier)
    {
        solutionPrice.BillingPeriod = TimeUnit.PerYear;
        solutionPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;

        tier.LowerRange = 0;
        tier.UpperRange = null;

        solutionPrice.Tiers = new List<CompetitionCatalogueItemPriceTier> { tier };

        model.SolutionDisplay.Price = solutionPrice;
        model.SolutionDisplay.Quantity = 1;

        var result = model.CalculateTotalYearlyCost();

        result.Should().Be(tier.Price);
    }
}
