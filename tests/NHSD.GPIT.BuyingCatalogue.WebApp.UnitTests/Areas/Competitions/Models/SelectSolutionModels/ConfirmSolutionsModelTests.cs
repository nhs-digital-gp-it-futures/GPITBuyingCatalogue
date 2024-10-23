using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.SelectSolutionModels;

public sealed class ConfirmSolutionsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        string competitionName,
        List<CompetitionSolution> competitionSolutions)
    {
        var shortlistedSolutions = competitionSolutions.Take(1).ToList();
        var nonShortlistedSolutions = competitionSolutions.Skip(1).Take(1).ToList();

        shortlistedSolutions.ForEach(x => x.IsShortlisted = true);
        nonShortlistedSolutions.ForEach(x => x.IsShortlisted = false);

        var solutions = shortlistedSolutions.Concat(nonShortlistedSolutions).ToList();
        var model = new ConfirmSolutionsModel(competitionName, solutions);

        model.CompetitionName.Should().Be(competitionName);
        model.CompetitionSolutions.Should().BeEquivalentTo(solutions);
        model.ShortlistedSolutions.Should().BeEquivalentTo(shortlistedSolutions);
        model.NonShortlistedSolutions.Should().BeEquivalentTo(nonShortlistedSolutions);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NullSolutions_SetsProperties(
        string competitionName)
    {
        var model = new ConfirmSolutionsModel(competitionName, null);

        model.CompetitionSolutions.Should().BeNull();
        model.ShortlistedSolutions.Should().BeNull();
        model.NonShortlistedSolutions.Should().BeNull();
    }
}
