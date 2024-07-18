using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public static class FeaturesRequirementsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        List<FeaturesCriteria> featuresCriteria)
    {
        competition.NonPriceElements = new() { Features = featuresCriteria };

        var model = new FeaturesRequirementsModel(competition);

        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);
        model.Features.Should().BeEquivalentTo(competition.NonPriceElements.Features);
    }
}
