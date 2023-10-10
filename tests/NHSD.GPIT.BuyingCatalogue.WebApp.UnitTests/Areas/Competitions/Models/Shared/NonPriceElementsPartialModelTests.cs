using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
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
        object routeValues)
    {
        var model = new NonPriceElementsPartialModel(internalOrgId, competitionId, nonPriceElements, routeValues);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.CompetitionId.Should().Be(competitionId);
        model.NonPriceElements.Should().Be(nonPriceElements);
        model.RouteValues.Should().Be(routeValues);
    }

    [Theory]
    [CommonAutoData]
    public static void GetIm1Integrations_Returns(
        string internalOrgId,
        int competitionId,
        NonPriceElements nonPriceElements,
        object routeValues)
    {
        var im1Integrations = Interoperability.Im1Integrations
            .Select(x => new InteroperabilityCriteria(x.Key, InteropIntegrationType.Im1))
            .ToList();

        nonPriceElements.Interoperability = im1Integrations;

        var model = new NonPriceElementsPartialModel(internalOrgId, competitionId, nonPriceElements, routeValues);

        var actualIntegrations = model.GetIm1Integrations();

        actualIntegrations.Should().HaveCount(im1Integrations.Count);
        actualIntegrations
            .Should()
            .BeEquivalentTo(im1Integrations.Select(x => Interoperability.Im1Integrations[x.Qualifier]));
    }

    [Theory]
    [CommonAutoData]
    public static void GetGpConnectIntegrations_Returns(
        string internalOrgId,
        int competitionId,
        NonPriceElements nonPriceElements,
        object routeValues)
    {
        var gpConnectIntegrations = Interoperability.GpConnectIntegrations
            .Select(x => new InteroperabilityCriteria(x.Key, InteropIntegrationType.GpConnect))
            .ToList();

        nonPriceElements.Interoperability = gpConnectIntegrations;

        var model = new NonPriceElementsPartialModel(internalOrgId, competitionId, nonPriceElements, routeValues);

        var actualIntegrations = model.GetGpConnectIntegrations();

        actualIntegrations.Should().HaveCount(gpConnectIntegrations.Count);
        actualIntegrations
            .Should()
            .BeEquivalentTo(gpConnectIntegrations.Select(x => Interoperability.GpConnectIntegrations[x.Qualifier]));
    }
}
