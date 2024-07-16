using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionResultsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionResultsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Confirm_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new ConfirmResultsModel(competition) { InternalOrgId = internalOrgId };

        var result = (await controller.Confirm(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task ViewResults_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IManageFiltersService filtersService,
        CompetitionResultsController controller)
    {
        competition.Organisation = organisation;

        filtersService.GetFilterDetails(Arg.Any<int>(), competition.FilterId).Returns(filterDetailsModel);

        competitionsService.GetCompetitionForResults(organisation.InternalIdentifier, competition.Id).Returns(competition);

        competitionsService.GetNonShortlistedSolutions(organisation.InternalIdentifier, competition.Id).Returns(nonShortlistedSolutions);

        var expectedModel = new ViewResultsModel(competition, filterDetailsModel, nonShortlistedSolutions);

        var result = (await controller.ViewResults(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task DirectAward_OneResult_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        FilterDetailsModel filterDetailsModel,
        ICollection<CompetitionSolution> nonShortlistedSolutions,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IManageFiltersService filtersService,
        CompetitionResultsController controller,
        Organisation organisation)
    {
        filtersService.GetFilterDetails(Arg.Any<int>(), competition.FilterId).Returns(filterDetailsModel);

        competitionsService.GetCompetitionForResults(internalOrgId, competition.Id).Returns(competition);

        competitionsService.GetNonShortlistedSolutions(internalOrgId, competition.Id).Returns(nonShortlistedSolutions);

        competition.Organisation = organisation;

        var expectedModel = new FilteredDirectAwardModel(competition, filterDetailsModel, nonShortlistedSolutions);

        var result = (await controller.DirectAward(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.PdfUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadResults_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.DownloadResults(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadResults_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IPdfService pdfService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competition.Id).Returns(competition);

        pdfService.BaseUri().Returns(new Uri("http://localhost"));

        var result = (await controller.DownloadResults(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be($"competition-results-{competition.Id}.pdf");
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadConfirmResults_NullCompetition_ReturnsNotFoundResult(
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competitionId).Returns((Competition)null);

        var result = (await controller.DownloadConfirmResults(internalOrgId, competitionId)).As<NotFoundResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadConfirmResults_ValidCompetition_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IPdfService pdfService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competition.Id).Returns(competition);

        pdfService.BaseUri().Returns(new Uri("http://localhost"));

        var result = (await controller.DownloadConfirmResults(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.FileDownloadName.Should().Be("review-scoring.pdf");
    }

    [Theory]
    [MockAutoData]
    public static async Task OrderingInformation_NoWinningSolution_Redirects(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.OrderingInformation(internalOrgId, competition.Id)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewResults));
    }

    [Theory]
    [MockAutoData]
    public static async Task OrderingInformation_InvalidSolutionId_Redirects(
        string internalOrgId,
        Competition competition,
        CatalogueItemId solutionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competitionsService.GetCompetitionForResults(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.OrderingInformation(internalOrgId, competition.Id, solutionId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewResults));
    }

    [Theory]
    [MockAutoData]
    public static async Task OrderingInformation_SetsCorrectBacklink(
        Organisation organisation,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IUrlHelper urlHelper,
        CompetitionResultsController controller)
    {
        competitionSolution.IsWinningSolution = true;
        competitionSolution.IsShortlisted = true;
        competitionSolution.Solution = solution;

        competition.Organisation = organisation;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionForResults(organisation.InternalIdentifier, competition.Id).Returns(competition);

        _ = await controller.OrderingInformation(
            organisation.InternalIdentifier,
            competition.Id);

        urlHelper.Received().Action(Arg.Is<UrlActionContext>(y => y.Action == nameof(controller.ViewResults)));
    }

    [Theory]
    [MockAutoData]
    public static async Task OrderingInformation_Valid_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competitionSolution.IsWinningSolution = true;
        competitionSolution.IsShortlisted = true;
        competitionSolution.Solution = solution;

        competition.Organisation = organisation;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionForResults(organisation.InternalIdentifier, competition.Id).Returns(competition);

        var expectedModel = new OrderingInformationModel(competition, competitionSolution);

        var result = (await controller.OrderingInformation(
            organisation.InternalIdentifier,
            competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_OrderingInformation_Redirects(
        string internalOrgId,
        int competitionId,
        OrderingInformationModel model,
        CatalogueItemId solutionId,
        CompetitionResultsController controller)
    {
        var result = (await controller.OrderingInformation(internalOrgId, competitionId, model, solutionId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OrderController.Order));
        result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
    }

    [Theory]
    [MockAutoData]
    public static async Task RecipientsCsv_ReturnsFileResult(
        string internalOrgId,
        Competition competition,
        List<OdsOrganisation> competitionRecipients,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionResultsController controller)
    {
        competition.Recipients = competitionRecipients;

        competitionsService.GetCompetitionWithRecipients(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.RecipientsCsv(internalOrgId, competition.Id)).As<FileResult>();

        result.Should().NotBeNull();
        result.ContentType.Should().Be("application/octet-stream");
    }
}
