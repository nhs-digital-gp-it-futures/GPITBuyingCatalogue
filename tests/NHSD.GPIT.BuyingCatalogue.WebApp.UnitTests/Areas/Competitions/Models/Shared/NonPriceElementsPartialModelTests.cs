using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.Shared;

public static class NonPriceElementsPartialModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string internalOrgId,
        int competitionId,
        NonPriceElements nonPriceElements,
        object routeValues,
        bool hasReviewedCriteria,
        Dictionary<SupportedIntegrations, string> integrations)
    {
        var model = new NonPriceElementsPartialModel(internalOrgId, competitionId, nonPriceElements, routeValues, hasReviewedCriteria, integrations);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.CompetitionId.Should().Be(competitionId);
        model.NonPriceElements.Should().Be(nonPriceElements);
        model.RouteValues.Should().Be(routeValues);
        model.IsReviewScreen.Should().BeFalse();
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_Alternate_SetsPropertiesAsExpected(
        string internalOrgId,
        int competitionId,
        NonPriceElements nonPriceElements,
        object routeValues,
        bool hasReviewedCriteria,
        Dictionary<SupportedIntegrations, string> integrations)
    {
        var model = new NonPriceElementsPartialModel(internalOrgId, competitionId, nonPriceElements, routeValues, hasReviewedCriteria, integrations);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.CompetitionId.Should().Be(competitionId);
        model.NonPriceElements.Should().Be(nonPriceElements);
        model.RouteValues.Should().Be(routeValues);
    }
}
