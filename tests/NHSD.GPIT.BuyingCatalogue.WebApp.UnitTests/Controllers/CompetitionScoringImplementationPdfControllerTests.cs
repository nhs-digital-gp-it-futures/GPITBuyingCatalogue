﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionScoringImplementationPdf;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers;

public static class CompetitionScoringImplementationPdfControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionScoringImplementationPdfController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringImplementationPdfController controller)
    {
        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.Index(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringImplementationPdfController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Implementation = new() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new PdfScoringImplementationModel(competition);

        var result = (await controller.Index(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
    }
}
