using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionShortlistedSolutionsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Solution solution,
        Competition competition,
        AdditionalService requiredService,
        List<CompetitionSolution> competitionSolutions,
        List<SolutionService> requiredServices)
    {
        requiredServices.ForEach(x =>
        {
            x.IsRequired = true;
            x.Service = requiredService.CatalogueItem;
        });

        competitionSolutions.ForEach(
            x =>
            {
                x.Solution = solution;
                x.SolutionServices = requiredServices;
            });

        competition.CompetitionSolutions = competitionSolutions;

        var model = new CompetitionShortlistedSolutionsModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.Solutions.Should().BeEquivalentTo(competitionSolutions.Select(x => new SolutionModel(x)));
    }
}
