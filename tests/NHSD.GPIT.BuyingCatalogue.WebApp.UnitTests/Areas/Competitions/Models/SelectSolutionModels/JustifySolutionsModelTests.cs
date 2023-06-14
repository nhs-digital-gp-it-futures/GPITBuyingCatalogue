using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.SelectSolutionModels;

public static class JustifySolutionsModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        string competitionName,
        Solution solution,
        CatalogueItem catalogueItem,
        List<CompetitionSolution> nonShortlistedSolutions)
    {
        solution.CatalogueItem = catalogueItem;

        nonShortlistedSolutions.ForEach(x => x.Solution = solution);

        var model = new JustifySolutionsModel(competitionName, nonShortlistedSolutions);

        model.CompetitionName.Should().Be(competitionName);
        model.Solutions.Should()
            .BeEquivalentTo(
                nonShortlistedSolutions.Select(x => new SolutionJustificationModel(x.Solution.CatalogueItem, x.Justification)));
    }
}
