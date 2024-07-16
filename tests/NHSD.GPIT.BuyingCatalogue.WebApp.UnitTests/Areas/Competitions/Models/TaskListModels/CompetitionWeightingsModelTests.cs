using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionWeightingsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Competition competition,
        Weightings weightings)
    {
        competition.Weightings = weightings;

        var model = new CompetitionWeightingsModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.Price.Should().Be(weightings.Price);
        model.NonPrice.Should().Be(weightings.NonPrice);
    }
}
