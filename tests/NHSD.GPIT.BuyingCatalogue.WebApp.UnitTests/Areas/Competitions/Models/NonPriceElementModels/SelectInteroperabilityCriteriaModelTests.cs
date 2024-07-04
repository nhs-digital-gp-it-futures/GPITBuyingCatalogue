using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class SelectInteroperabilityCriteriaModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        List<Integration> integrations)
    {
        var model = new SelectInteroperabilityCriteriaModel(competition, integrations);

        model.CompetitionName.Should().Be(competition.Name);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithIntegrations_SetsPropertiesAsExpected(
        Competition competition,
        List<IntegrationType> integrationTypes,
        List<Integration> integrations)
    {
        integrations.ForEach(x => x.IntegrationTypes = integrationTypes);

        var model = new SelectInteroperabilityCriteriaModel(competition, integrations);

        var expectedIntegrations = integrations.Select(
                x => new KeyValuePair<string, List<SelectOption<int>>>(
                    x.Name,
                    x.IntegrationTypes.Select(y => new SelectOption<int>(y.Name, y.Id, false)).ToList()))
            .ToList();

        model.Integrations.Should().BeEquivalentTo(expectedIntegrations);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithSelectedIntegrationTypes_SetsPropertiesAsExpected(
        Competition competition,
        List<IntegrationType> integrationTypes,
        List<Integration> integrations)
    {
        competition.NonPriceElements = new() { IntegrationTypes = integrationTypes };
        integrations.ForEach(x => x.IntegrationTypes = integrationTypes);

        var model = new SelectInteroperabilityCriteriaModel(competition, integrations);

        var expectedIntegrations = integrations.Select(
                x => new KeyValuePair<string, List<SelectOption<int>>>(
                    x.Name,
                    x.IntegrationTypes.Select(y => new SelectOption<int>(y.Name, y.Id, true)).ToList()))
            .ToList();

        model.Integrations.Should().BeEquivalentTo(expectedIntegrations);
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_NoIntegrationTypes_ReturnsTrue(
        Competition competition,
        List<Integration> integrations)
    {
        competition.NonPriceElements = new() { IntegrationTypes = new List<IntegrationType>() };

        var model = new SelectInteroperabilityCriteriaModel(competition, integrations);

        model.CanDelete.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_WithIntegrationTypes_ReturnsFalse(
        List<IntegrationType> integrationTypes,
        Competition competition,
        List<Integration> integrations)
    {
        competition.NonPriceElements = new() { IntegrationTypes = integrationTypes };

        var model = new SelectInteroperabilityCriteriaModel(competition, integrations);

        model.CanDelete.Should().BeFalse();
    }
}
