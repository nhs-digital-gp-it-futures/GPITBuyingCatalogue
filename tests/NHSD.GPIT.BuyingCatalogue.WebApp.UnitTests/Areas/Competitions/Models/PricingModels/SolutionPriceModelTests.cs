using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.PricingModels;

public static class SolutionPriceModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution)
    {
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };
        competitionSolution.Solution = solution;

        var model = new SolutionPriceModel(competitionSolution, competition);

        model.Name.Should().Be(solution.CatalogueItem.Name);
        model.Price.Should().BeNull();
        model.Progress.Should().Be(TaskProgress.NotStarted);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_PriceProgressCompleted_SetsPrice(
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier tier,
        int quantity)
    {
        tier.LowerRange = 0;
        tier.UpperRange = null;

        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { tier };

        competitionSolution.Solution = solution;
        competitionSolution.Quantity = quantity;
        competitionSolution.Price = price;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        var expectedPrice = competitionSolution.CalculateTotalPrice(competition.ContractLength.GetValueOrDefault());

        var model = new SolutionPriceModel(competitionSolution, competition);

        model.Progress.Should().Be(TaskProgress.Completed);
        model.Price.Should().Be(expectedPrice);
    }
}
