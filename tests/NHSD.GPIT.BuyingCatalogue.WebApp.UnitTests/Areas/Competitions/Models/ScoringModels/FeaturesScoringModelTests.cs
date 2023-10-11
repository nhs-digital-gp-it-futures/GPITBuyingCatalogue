using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class FeaturesScoringModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        List<FeaturesCriteria> featuresCriteria)
    {
        var mustFeature = featuresCriteria.First();
        var shouldFeature = featuresCriteria.Skip(1).First();

        mustFeature.Compliance = CompliancyLevel.Must;
        shouldFeature.Compliance = CompliancyLevel.Should;

        competition.NonPriceElements = new() { Features = featuresCriteria };

        var model = new FeaturesScoringModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.Features.Should().BeEquivalentTo(featuresCriteria);
        model.MustFeatures.Should().Contain(mustFeature);
        model.ShouldFeatures.Should().Contain(shouldFeature);
    }
}
