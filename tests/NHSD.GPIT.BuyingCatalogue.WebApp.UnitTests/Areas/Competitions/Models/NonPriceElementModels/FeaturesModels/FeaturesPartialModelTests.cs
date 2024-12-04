using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared.Partials;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public static class FeaturesPartialModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string internalOrgId,
        int competitionId,
        List<FeaturesCriteria> featuresCriteria,
        bool hasReviewedCriteria)
    {
        var shouldFeature = featuresCriteria.First();
        var mustFeature = featuresCriteria.Skip(1).First();

        shouldFeature.Compliance = CompliancyLevel.Should;
        mustFeature.Compliance = CompliancyLevel.Must;

        var model = new FeaturesPartialModel(internalOrgId, competitionId, featuresCriteria, hasReviewedCriteria);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.CompetitionId.Should().Be(competitionId);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_Alternate_SetsPropertiesAsExpected(
        string internalOrgId,
        int competitionId,
        List<FeaturesCriteria> featuresCriteria,
        bool hasReviewedCriteria)
    {
        var model = new FeaturesPartialModel(
            internalOrgId,
            competitionId,
            featuresCriteria,
            hasReviewedCriteria);
    }

    [Fact]
    public static void CanDelete_NullReturnUrl_NullRequirements_True()
    {
        var model = new FeaturesPartialModel();
        model.CanDelete.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_ReturnUrl_MultipleRequirements_True(
        string returnUrl,
        FeaturesCriteria featuresCriteria1,
        FeaturesCriteria featuresCriteria2)
    {
        var model = new FeaturesPartialModel
        {
            ReturnUrl = returnUrl,
            Requirements =
            [
                featuresCriteria1,
                featuresCriteria2,
            ],
        };

        model.CanDelete.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_ReturnUrl_SingleRequirement_False(
        string returnUrl,
        FeaturesCriteria featuresCriteria1)
    {
        var model = new FeaturesPartialModel
        {
            ReturnUrl = returnUrl,
            Requirements =
            [
                featuresCriteria1,
            ],
        };

        model.CanDelete.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_ReturnUrl_NullRequirement_False(
        string returnUrl)
    {
        var model = new FeaturesPartialModel
        {
            ReturnUrl = returnUrl,
        };

        model.CanDelete.Should().BeFalse();
    }
}
