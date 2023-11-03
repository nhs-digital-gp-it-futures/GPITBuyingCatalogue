using System;
using System.Collections.Generic;
using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionScoringControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionScoringController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new NonPriceElementScoresDashboardModel(competition);

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Interoperability_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Interoperability = Enumerable.Empty<InteroperabilityCriteria>().ToList() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new InteroperabilityScoringModel(competition);

        var result = (await controller.Interoperability(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Interoperability_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        InteroperabilityScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Interoperability = Enumerable.Empty<InteroperabilityCriteria>().ToList() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.Interoperability(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Interoperability_ValidModel_SetsInteroperabilityScores(
        string internalOrgId,
        int competitionId,
        InteroperabilityScoringModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.Interoperability(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetSolutionsInteroperabilityScores(
                internalOrgId,
                competitionId,
                It.IsAny<Dictionary<CatalogueItemId, (int, string)>>()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Implementation_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Implementation = new() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new ImplementationScoringModel(competition);

        var result = (await controller.Implementation(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Implementation_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        ImplementationScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Implementation = new() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.Implementation(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Implementation_ValidModel_SetsImplementationScores(
        string internalOrgId,
        int competitionId,
        ImplementationScoringModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.Implementation(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetSolutionsImplementationScores(
                internalOrgId,
                competitionId,
                It.IsAny<Dictionary<CatalogueItemId, (int, string)>>()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { ServiceLevel = new() { ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>() } };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new ServiceLevelScoringModel(competition);

        var result = (await controller.ServiceLevel(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevel_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        ServiceLevelScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { ServiceLevel = new() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.ServiceLevel(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevel_ValidModel_SetsServiceLevelScores(
        string internalOrgId,
        int competitionId,
        ServiceLevelScoringModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.ServiceLevel(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetSolutionsServiceLevelScores(
                internalOrgId,
                competitionId,
                It.IsAny<Dictionary<CatalogueItemId, (int, string)>>()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Features_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria>() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new FeaturesScoringModel(competition);

        var result = (await controller.Features(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Features_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        FeaturesScoringModel model,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        competitionSolutions.ForEach(x => x.Solution = solution);
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria>() };

        competitionsService.Setup(x => x.GetCompetitionWithSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.Features(internalOrgId, competition.Id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Features_ValidModel_SetsFeaturesScores(
        string internalOrgId,
        int competitionId,
        FeaturesScoringModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        var result = (await controller.Features(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetSolutionsFeaturesScores(
                internalOrgId,
                competitionId,
                It.IsAny<Dictionary<CatalogueItemId, (int, string)>>()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeaturePdf_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        string pdfType = "feature";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.GeneratePdf(internalOrgId, competitionId, pdfType)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeaturePdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IPdfService> pdfService,
        CompetitionScoringController controller)
    {
        string pdfType = "feature";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        pdfService
            .Setup(s => s.BaseUri())
            .Returns(new Uri("http://localhost"));

        var result = (await controller.GeneratePdf(internalOrgId, competition.Id, pdfType)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-features.pdf");
    }

    [Theory]
    [CommonAutoData]
    public static async Task ImplementationPdf_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionScoringController controller)
    {
        string pdfType = "implementation";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.GeneratePdf(internalOrgId, competitionId, pdfType)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task ImplementationPdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IPdfService> pdfService,
        CompetitionScoringController controller)
    {
        string pdfType = "implementation";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        pdfService
            .Setup(s => s.BaseUri())
            .Returns(new Uri("http://localhost"));

        var result = (await controller.GeneratePdf(internalOrgId, competition.Id, pdfType)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-implementation.pdf");
    }

    [Theory]
    [CommonAutoData]
    public static async Task InteropPdf_NullCompetition_ReturnsNotFoundResult(
       string internalOrgId,
       int competitionId,
       [Frozen] Mock<ICompetitionsService> competitionsService,
       CompetitionScoringController controller)
    {
        string pdfType = "interop";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.GeneratePdf(internalOrgId, competitionId, pdfType)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task InteropPdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IPdfService> pdfService,
        CompetitionScoringController controller)
    {
        string pdfType = "interop";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        pdfService
            .Setup(s => s.BaseUri())
            .Returns(new Uri("http://localhost"));

        var result = (await controller.GeneratePdf(internalOrgId, competition.Id, pdfType)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-interoperability.pdf");
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevelPdf_NullCompetition_ReturnsNotFoundResult(
       string internalOrgId,
       int competitionId,
       [Frozen] Mock<ICompetitionsService> competitionsService,
       CompetitionScoringController controller)
    {
        string pdfType = "serviceLevel";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.GeneratePdf(internalOrgId, competitionId, pdfType)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevelPdf_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IPdfService> pdfService,
        CompetitionScoringController controller)
    {
        string pdfType = "serviceLevel";
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        pdfService
            .Setup(s => s.BaseUri())
            .Returns(new Uri("http://localhost"));

        var result = (await controller.GeneratePdf(internalOrgId, competition.Id, pdfType)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("compare-sla.pdf");
    }
}
