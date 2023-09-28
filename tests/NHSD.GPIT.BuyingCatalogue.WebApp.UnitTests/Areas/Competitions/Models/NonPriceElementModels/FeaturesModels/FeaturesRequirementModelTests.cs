using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public static class FeaturesRequirementModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition)
    {
        var model = new FeaturesRequirementModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_WithFeaturesCriteria_SetsPropertiesAsExpected(
        Competition competition,
        FeaturesCriteria featuresCriteria)
    {
        var model = new FeaturesRequirementModel(competition, featuresCriteria);

        model.CompetitionName.Should().Be(competition.Name);
        model.FeaturesCriteriaId.Should().Be(featuresCriteria.Id);
        model.Requirements.Should().Be(featuresCriteria.Requirements);
        model.SelectedCompliance.Should().Be(featuresCriteria.Compliance);
    }
}
