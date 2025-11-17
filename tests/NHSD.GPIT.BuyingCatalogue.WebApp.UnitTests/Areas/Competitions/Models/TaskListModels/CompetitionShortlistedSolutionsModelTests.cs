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
        string frameworkName,
        AdditionalService requiredService,
        List<CompetitionSolution> competitionSolutions,
        List<CompetitionAdditionalService> requiredServices)
    {
        requiredServices.ForEach(x =>
        {
            x.CatalogueItem = requiredService.CatalogueItem;
        });

        competitionSolutions.ForEach(
            x =>
            {
                x.CatalogueItem = solution.CatalogueItem;
                x.Services = requiredServices.Cast<CompetitionCatalogueItem>().ToList();
            });

        competition.CompetitionSolutions = competitionSolutions;

        var model = new CompetitionShortlistedSolutionsModel(competition, frameworkName);

        model.CompetitionName.Should().Be(competition.Name);
        model.FrameworkName.Should().Be(frameworkName);
        model.Solutions.Should().BeEquivalentTo(competitionSolutions.Select(x => new SolutionModel(x)));
    }
}
