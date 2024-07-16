using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionAwardCriteriaModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(Competition competition)
    {
        var model = new CompetitionAwardCriteriaModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.IncludesNonPrice.Should().Be(competition.IncludesNonPrice);
    }
}
