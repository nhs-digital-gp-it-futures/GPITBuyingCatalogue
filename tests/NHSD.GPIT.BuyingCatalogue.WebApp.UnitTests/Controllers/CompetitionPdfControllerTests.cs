using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionPdfModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers;

public static class CompetitionPdfControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionPdfController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.ConfirmResults(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new PdfConfirmResultsModel(competition);

        var result = (await controller.ConfirmResults(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task GetScoringView_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.GetScoringView(internalOrgId, competitionId, c => new StubModel(c))).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task GetScoringView_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.GetScoringView(internalOrgId, competition.Id, c => new StubModel(c))).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task ImplementationScoring_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Implementation = new() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new PdfScoringImplementationModel(competition);

        var result = (await controller.ImplementationScoring(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task InteropScoring_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IIntegrationsService integrationsService,
        CompetitionPdfController controller,
        List<Integration> integrations)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { IntegrationTypes = Enumerable.Empty<IntegrationType>().ToList() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        integrationsService.GetIntegrations().Returns(integrations);

        var expectedModel = new PdfScoringInteropModel(competition, integrations);

        var result = (await controller.InteropScoring(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevelScoring_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { ServiceLevel = new() { ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>() } };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new PdfScoringServiceLevelModel(competition);

        var result = (await controller.ServiceLevelScoring(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task FeaturesScoring_ValidCompetition_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionPdfController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria>(), };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new PdfFeaturesScoringModel(competition);

        var result = (await controller.FeaturesScoring(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    private class StubModel
    {
        public StubModel(Competition competition)
        {
            _ = competition;
        }
    }
}
