using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;

public static class OrderingInformationModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        List<OdsOrganisation> recipients)
    {
        competition.Recipients = recipients;
        competition.Organisation = organisation;
        competitionSolution.Solution = solution;

        var model = new OrderingInformationModel(competition, competitionSolution);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);

        model.NumberOfRecipients.Should().Be(recipients.Count);
        model.ContractLength.Should().Be(competition.ContractLength);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SolutionGlobalQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        List<OdsOrganisation> recipients,
        int globalQuantity)
    {
        competition.Recipients = recipients;
        competition.Organisation = organisation;
        competitionSolution.Solution = solution;
        competitionSolution.Quantity = globalQuantity;

        var model = new OrderingInformationModel(competition, competitionSolution);

        model.SolutionDisplay.Should()
            .BeEquivalentTo(
                new OrderingInformationItem(solution.CatalogueItem, competitionSolution.Price, globalQuantity));
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SolutionRecipientQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        List<OdsOrganisation> recipients,
        int recipientQuantity)
    {
        competition.Recipients = recipients;
        competition.Organisation = organisation;
        competitionSolution.Solution = solution;
        competitionSolution.Quantity = null;
        competitionSolution.Quantities = recipients
            .Select(x => new SolutionQuantity { OdsCode = x.Id, Quantity = recipientQuantity })
            .ToList();

        var model = new OrderingInformationModel(competition, competitionSolution);

        model.SolutionDisplay.Should()
            .BeEquivalentTo(
                new OrderingInformationItem(solution.CatalogueItem, competitionSolution.Price, competitionSolution.Quantities.Sum(x => x.Quantity)));
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SolutionServiceGlobalQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        SolutionService solutionService,
        List<OdsOrganisation> recipients,
        int globalQuantity)
    {
        competition.Recipients = recipients;
        competition.Organisation = organisation;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        solutionService.Service = additionalService.CatalogueItem;
        solutionService.Quantity = globalQuantity;

        var model = new OrderingInformationModel(competition, competitionSolution);

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
    [CommonAutoData]
    public static void Construct_SolutionServiceRecipientQuantity_SetsAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        SolutionService solutionService,
        List<OdsOrganisation> recipients,
        int recipientQuantity)
    {
        competition.Recipients = recipients;
        competition.Organisation = organisation;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        solutionService.Service = additionalService.CatalogueItem;
        solutionService.Quantity = null;
        solutionService.Quantities = recipients
            .Select(x => new ServiceQuantity() { OdsCode = x.Id, Quantity = recipientQuantity })
            .ToList();

        var model = new OrderingInformationModel(competition, competitionSolution);

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
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
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
    [CommonAutoData]
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
