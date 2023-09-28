using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public static class FeaturesRequirementsPartialModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string internalOrgId,
        int competitionId,
        List<FeaturesCriteria> featuresCriteria)
    {
        var shouldFeature = featuresCriteria.First();
        var mustFeature = featuresCriteria.Skip(1).First();

        shouldFeature.Compliance = CompliancyLevel.Should;
        mustFeature.Compliance = CompliancyLevel.Must;

        var model = new FeaturesRequirementsPartialModel(internalOrgId, competitionId, featuresCriteria);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.CompetitionId.Should().Be(competitionId);
        model.MustRequirements.Should().Contain(mustFeature);
        model.ShouldRequirements.Should().Contain(shouldFeature);
    }
}
