using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionScoringControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionScoringController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id).Returns(competition);

        var expectedModel = new NonPriceElementScoresDashboardModel(competition);

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Interoperability_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        List<Integration> integrations,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IIntegrationsService integrationsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { IntegrationTypes = Enumerable.Empty<IntegrationType>().ToList() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id)
            .Returns(competition);

        integrationsService.GetIntegrations().Returns(integrations);

        var expectedModel = new InteroperabilityScoringModel(competition, integrations);

        var result = (await controller.Interoperability(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task Interoperability_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        InteroperabilityScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { IntegrationTypes = Enumerable.Empty<IntegrationType>().ToList() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.Interoperability(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Interoperability_ValidModel_SetsInteroperabilityScores(
        string internalOrgId,
        int competitionId,
        InteroperabilityScoringModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.Interoperability(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetSolutionsInteroperabilityScores(
                internalOrgId,
                competitionId,
                Arg.Any<Dictionary<CatalogueItemId, (int, string)>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Implementation_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Implementation = new() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new ImplementationScoringModel(competition);

        var result = (await controller.Implementation(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task Implementation_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        ImplementationScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Implementation = new() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.Implementation(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Implementation_ValidModel_SetsImplementationScores(
        string internalOrgId,
        int competitionId,
        ImplementationScoringModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.Implementation(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetSolutionsImplementationScores(
                internalOrgId,
                competitionId,
                Arg.Any<Dictionary<CatalogueItemId, (int, string)>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { ServiceLevel = new() { ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>() } };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new ServiceLevelScoringModel(competition);

        var result = (await controller.ServiceLevel(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevel_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        ServiceLevelScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { ServiceLevel = new() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.ServiceLevel(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevel_ValidModel_SetsServiceLevelScores(
        string internalOrgId,
        int competitionId,
        ServiceLevelScoringModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.ServiceLevel(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetSolutionsServiceLevelScores(
                internalOrgId,
                competitionId,
                Arg.Any<Dictionary<CatalogueItemId, (int, string)>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Features_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria>() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new FeaturesScoringModel(competition);

        var result = (await controller.Features(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task Features_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        FeaturesScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria>() };

        competitionsService.GetCompetitionWithSolutions(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.Features(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Features_ValidModel_SetsFeaturesScores(
        string internalOrgId,
        int competitionId,
        FeaturesScoringModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.Features(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetSolutionsFeaturesScores(
                internalOrgId,
                competitionId,
                Arg.Any<Dictionary<CatalogueItemId, (int, string)>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task FeaturePdf_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.FeaturePdf(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task FeaturePdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IPdfService pdfService,
        CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competition.Id).Returns(competition);

        pdfService.BaseUri().Returns(new Uri("http://localhost"));

        var result = (await controller.FeaturePdf(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-features.pdf");
    }

    [Theory]
    [MockAutoData]
    public static async Task ImplementationPdf_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.ImplementationPdf(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task ImplementationPdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IPdfService pdfService,
        CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competition.Id).Returns(competition);

        pdfService.BaseUri().Returns(new Uri("http://localhost"));

        var result = (await controller.ImplementationPdf(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-implementation.pdf");
    }

    [Theory]
    [MockAutoData]
    public static async Task InteropPdf_NullCompetition_ReturnsNotFoundResult(
       string internalOrgId,
       int competitionId,
       [Frozen] ICompetitionsService competitionsService,
       CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.InteropPdf(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task InteropPdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IPdfService pdfService,
        CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competition.Id).Returns(competition);

        pdfService.BaseUri().Returns(new Uri("http://localhost"));

        var result = (await controller.InteropPdf(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-interoperability.pdf");
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevelPdf_NullCompetition_ReturnsNotFoundResult(
       string internalOrgId,
       int competitionId,
       [Frozen] ICompetitionsService competitionsService,
       CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.ServiceLevelPdf(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevelPdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IPdfService pdfService,
        CompetitionScoringController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competition.Id).Returns(competition);

        pdfService.BaseUri().Returns(new Uri("http://localhost"));

        var result = (await controller.ServiceLevelPdf(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-sla.pdf");
    }
}
