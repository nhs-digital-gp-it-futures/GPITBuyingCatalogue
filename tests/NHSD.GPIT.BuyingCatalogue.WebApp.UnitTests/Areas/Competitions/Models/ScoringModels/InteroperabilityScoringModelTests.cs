﻿using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class InteroperabilityScoringModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        Solution solution)
    {
        competition.CompetitionSolutions =
            new List<CompetitionSolution> { new(competition.Id, solution.CatalogueItemId) { Solution = solution } };
        competition.NonPriceElements = new()
        {
            Interoperability = new List<InteroperabilityCriteria>
            {
                new("Patient Facing", InteropIntegrationType.Im1),
                new("Structured Record", InteropIntegrationType.GpConnect),
            },
        };

        var model = new InteroperabilityScoringModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.InteroperabilityCriteria.Should().BeEquivalentTo(competition.NonPriceElements.Interoperability);
        model.SolutionScores.Should().ContainSingle();
    }

    [Theory]
    [CommonAutoData]
    public static void GetIm1Integrations_ReturnsExpected(
        Competition competition,
        Solution solution)
    {
        competition.CompetitionSolutions =
            new List<CompetitionSolution> { new(competition.Id, solution.CatalogueItemId) { Solution = solution } };
        competition.NonPriceElements = new()
        {
            Interoperability = new List<InteroperabilityCriteria>
            {
                new("Patient Facing", InteropIntegrationType.Im1),
                new("Structured Record", InteropIntegrationType.GpConnect),
            },
        };

        var model = new InteroperabilityScoringModel(competition);

        model.GetIm1Integrations().Should().ContainSingle();
    }

    [Theory]
    [CommonAutoData]
    public static void GetGpConnectIntegrations_ReturnsExpected(
        Competition competition,
        Solution solution)
    {
        competition.CompetitionSolutions =
            new List<CompetitionSolution> { new(competition.Id, solution.CatalogueItemId) { Solution = solution } };
        competition.NonPriceElements = new()
        {
            Interoperability = new List<InteroperabilityCriteria>
            {
                new("Patient Facing", InteropIntegrationType.Im1),
                new("Structured Record", InteropIntegrationType.GpConnect),
            },
        };

        var model = new InteroperabilityScoringModel(competition);

        model.GetGpConnectIntegrations().Should().ContainSingle();
    }
}
