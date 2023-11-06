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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionResultsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionResultsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Confirm_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.Setup(x => x.GetCompetitionForResults(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new ConfirmResultsModel(competition) { InternalOrgId = internalOrgId };

        var result = (await controller.Confirm(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Confirm_Post_Redirects(
        string internalOrgId,
        int competitionId,
        ConfirmResultsModel model,
        CompetitionResultsController controller)
    {
        var result = (await controller.Confirm(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewResults));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ViewResults_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IManageFiltersService> filtersService,
        CompetitionResultsController controller)
    {
        competition.Organisation = organisation;

        filtersService.Setup(x => x.GetFilterDetails(It.IsAny<int>(), competition.FilterId))
            .ReturnsAsync(filterDetailsModel);

        competitionsService.Setup(x => x.GetCompetitionForResults(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        competitionsService.Setup(x => x.GetNonShortlistedSolutions(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(nonShortlistedSolutions);

        var expectedModel = new ViewResultsModel(competition, filterDetailsModel, nonShortlistedSolutions);

        var result = (await controller.ViewResults(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task DirectAward_OneResult_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IManageFiltersService> filtersService,
        CompetitionResultsController controller,
        Organisation organisation)
    {
        filtersService.Setup(x => x.GetFilterDetails(It.IsAny<int>(), competition.FilterId))
            .ReturnsAsync(filterDetailsModel);

        competitionsService.Setup(x => x.GetCompetitionForResults(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        competitionsService.Setup(x => x.GetNonShortlistedSolutions(internalOrgId, competition.Id))
            .ReturnsAsync(nonShortlistedSolutions);

        competition.Organisation = organisation;

        var expectedModel = new FilteredDirectAwardModel(competition, filterDetailsModel, nonShortlistedSolutions);

        var result = (await controller.DirectAward(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [CommonAutoData]
    public static async Task DownloadResults_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.DownloadResults(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task DownloadResults_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IPdfService> pdfService,
        CompetitionResultsController controller)
    {
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        pdfService
            .Setup(s => s.BaseUri())
            .Returns(new Uri("http://localhost"));

        var result = (await controller.DownloadResults(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be($"competition-results-{competition.Id}.pdf");
    }

    [Theory]
    [CommonAutoData]
    public static async Task DownloadConfirmResults_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.Setup(x => x.GetCompetitionForResults(internalOrgId, competitionId))
            .ReturnsAsync((Competition)null);

        var result = (await controller.DownloadConfirmResults(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task DownloadConfirmResults_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IPdfService> pdfService,
        CompetitionResultsController controller)
    {
        competitionsService.Setup(x => x.GetCompetitionForResults(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        pdfService
            .Setup(s => s.BaseUri())
            .Returns(new Uri("http://localhost"));

        var result = (await controller.DownloadConfirmResults(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("review-scoring.pdf");
    }

    [Theory]
    [CommonAutoData]
    public static async Task SelectWinningSolution_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionResultsController controller)
    {
        var winningSolutions = competitionSolutions.Take(1).ToList();
        winningSolutions.ForEach(
            x =>
            {
                x.Solution = solution;
                x.IsWinningSolution = true;
            });

        competition.CompetitionSolutions = competitionSolutions;

        var expectedModel = new SelectWinningSolutionModel(competition.Name, winningSolutions.Select(x => x.Solution));

        competitionsService.Setup(x => x.GetCompetitionForResults(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.SelectWinningSolution(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task SelectWinningSolution_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        SelectWinningSolutionModel model,
        CompetitionResultsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectWinningSolution(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task SelectWinningSolution_ValidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        SelectWinningSolutionModel model,
        CompetitionResultsController controller)
    {
        var result = (await controller.SelectWinningSolution(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }
}
