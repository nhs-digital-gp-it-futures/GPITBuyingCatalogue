using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels;

public static class SelectWinningSolutionModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string competitionName,
        List<Solution> solutions)
    {
        var expectedWinningSolutions = solutions.Select(
                x => new SelectOption<CatalogueItemId>(
                    x.CatalogueItem.Name,
                    x.CatalogueItem.Supplier.LegalName,
                    x.CatalogueItemId))
            .ToList();

        var model = new SelectWinningSolutionModel(competitionName, solutions);

        model.CompetitionName.Should().Be(competitionName);
        model.WinningSolutions.Should().BeEquivalentTo(expectedWinningSolutions);
    }
}
