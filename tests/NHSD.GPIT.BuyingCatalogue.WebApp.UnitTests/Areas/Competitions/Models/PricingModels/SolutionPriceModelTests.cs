using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.PricingModels;

public static class SolutionPriceModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution,
        CompetitionSolution competitionSolution)
    {
        competitionSolution.Solution = solution;
        var model = new SolutionPriceModel(competitionSolution);

        model.Name.Should().Be(solution.CatalogueItem.Name);
        model.Price.Should().BeNull();
        model.Progress.Should().Be(TaskProgress.NotStarted);
    }
}
