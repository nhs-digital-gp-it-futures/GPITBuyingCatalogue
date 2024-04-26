﻿using System.Collections.Generic;
using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionSelectSolutionsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionSelectSolutionsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectSolutions_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        const bool shouldTrack = false;

        solution.FrameworkSolutions = new List<FrameworkSolution> { new FrameworkSolution() { FrameworkId = competition.FrameworkId, Solution = solution } };
        competitionSolutions.ForEach(
            x =>
            {
                x.Solution = solution;
                x.SolutionServices = new List<SolutionService>();
            });

        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithServicesAndFramework(organisation.InternalIdentifier, competition.Id, shouldTrack)
            .Returns(Task.FromResult(competition));

        var expectedModel = new SelectSolutionsModel(competition.Name, competition.CompetitionSolutions)
        {
            BackLinkText = "Go back to manage competitions",
        };

        var result =
            (await controller.SelectSolutions(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectSolutions_NullCompetition_RedirectsToDashboard(
        Organisation organisation,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        const bool shouldTrack = false;

        competitionsService.GetCompetitionWithServicesAndFramework(organisation.InternalIdentifier, competitionId, shouldTrack)
            .Returns(Task.FromResult((Competition)null));

        var result =
            (await controller.SelectSolutions(organisation.InternalIdentifier, competitionId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionsDashboardController.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectSolutions_NoSolutions_DeletesCompetition(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        const bool shouldTrack = false;

        competition.CompetitionSolutions = new List<CompetitionSolution>();

        competitionsService.GetCompetitionWithServicesAndFramework(organisation.InternalIdentifier, competition.Id, shouldTrack).Returns(Task.FromResult(competition));

        var expectedModel = new SelectSolutionsModel(competition.Name, competition.CompetitionSolutions)
        {
            BackLinkText = "Go back to manage competitions",
        };

        var result =
            (await controller.SelectSolutions(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        await competitionsService.Received(1).DeleteCompetition(organisation.InternalIdentifier, competition.Id);

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectSolutions_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        SelectSolutionsModel model,
        CompetitionSelectSolutionsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectSolutions(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectSolutions_SingleDirectAward_RedirectsToOrderDescription(
        Organisation organisation,
        int competitionId,
        SelectSolutionsModel model,
        SolutionModel solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        model.Solutions = new() { solution };
        model.IsDirectAward = true;

        var result = (await controller.SelectSolutions(organisation.InternalIdentifier, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received(1).CompleteCompetition(organisation.InternalIdentifier, competitionId, true);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OrderDescriptionController.NewOrderDescription));
        result.ControllerName.Should().Be(typeof(OrderDescriptionController).ControllerName());
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectSolutions_SingleCancelCompetition_RedirectsToCompetitionsDashboard(
        Organisation organisation,
        int competitionId,
        SelectSolutionsModel model,
        SolutionModel solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        model.Solutions = new() { solution };
        model.IsDirectAward = false;

        var result = (await controller.SelectSolutions(organisation.InternalIdentifier, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received(1).DeleteCompetition(organisation.InternalIdentifier, competitionId);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionsDashboardController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionsDashboardController).ControllerName());
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectSolutions_Valid_RedirectsToJustification(
        Organisation organisation,
        int competitionId,
        SelectSolutionsModel model,
        List<SolutionModel> solutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        model.Solutions = solutions;

        var result = (await controller.SelectSolutions(organisation.InternalIdentifier, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received(1).SetShortlistedSolutions(
                organisation.InternalIdentifier,
                competitionId,
                Arg.Any<IEnumerable<CatalogueItemId>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.JustifySolutions));
    }

    [Theory]
    [MockAutoData]
    public static async Task JustifySolutions_AllSolutionsShortlisted_RedirectsToConfirmation(
        Organisation organisation,
        Competition competition,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        competitionSolutions.ForEach(x => x.IsShortlisted = true);

        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithServices(organisation.InternalIdentifier, competition.Id, false)
            .Returns(Task.FromResult(competition));

        var result = (await controller.JustifySolutions(organisation.InternalIdentifier, competition.Id))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ConfirmSolutions));
    }

    [Theory]
    [MockAutoData]
    public static async Task JustifySolutions_NonShortlistedSolutions_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        Solution solution,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        var nonShortlistedSolutions = competitionSolutions.Take(1).ToList();
        competitionSolutions.Skip(1).ToList().ForEach(x => x.IsShortlisted = true);
        nonShortlistedSolutions.ForEach(x => x.IsShortlisted = false);

        competitionSolutions.ForEach(x => x.Solution = solution);

        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithServices(organisation.InternalIdentifier, competition.Id, false)
            .Returns(Task.FromResult(competition));

        var expectedModel = new JustifySolutionsModel(competition.Name, nonShortlistedSolutions);

        var result = (await controller.JustifySolutions(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_JustifySolutions_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        JustifySolutionsModel model,
        CompetitionSelectSolutionsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.JustifySolutions(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_JustifySolutions_Valid_RedirectsToConfirmSolutions(
        Organisation organisation,
        int competitionId,
        JustifySolutionsModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        var result = (await controller.JustifySolutions(organisation.InternalIdentifier, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received(1).SetSolutionJustifications(
                organisation.InternalIdentifier,
                competitionId,
                Arg.Any<Dictionary<CatalogueItemId, string>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ConfirmSolutions));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSolutions_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithServices(organisation.InternalIdentifier, competition.Id, false)
            .Returns(Task.FromResult(competition));

        var expectedModel = new ConfirmSolutionsModel(competition.Name, competitionSolutions);

        var result =
            (await controller.ConfirmSolutions(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSolutions_AllSolutionsShortlisted_UsesCorrectBacklink(
        Organisation organisation,
        Competition competition,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IUrlHelper urlHelper,
        CompetitionSelectSolutionsController controller)
    {
        competitionSolutions.ForEach(x => x.IsShortlisted = true);

        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithServices(organisation.InternalIdentifier, competition.Id, false)
            .Returns(Task.FromResult(competition));

        _ = (await controller.ConfirmSolutions(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        urlHelper.Received().Action(Arg.Is<UrlActionContext>(x => x.Action == nameof(controller.SelectSolutions)));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSolutions_WithNonShortlistedSolutions_UsesCorrectBacklink(
        Organisation organisation,
        Competition competition,
        List<CompetitionSolution> competitionSolutions,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IUrlHelper urlHelper,
        CompetitionSelectSolutionsController controller)
    {
        competitionSolutions.Take(1).ToList().ForEach(x => x.IsShortlisted = false);
        competitionSolutions.Skip(1).ToList().ForEach(x => x.IsShortlisted = true);

        competition.CompetitionSolutions = competitionSolutions;

        competitionsService.GetCompetitionWithServices(organisation.InternalIdentifier, competition.Id, false)
            .Returns(Task.FromResult(competition));

        _ = (await controller.ConfirmSolutions(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        urlHelper.Received().Action(Arg.Is<UrlActionContext>(x => x.Action == nameof(controller.JustifySolutions)));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_ConfirmSolutions_Redirects(
        Organisation organisation,
        int competitionId,
        ConfirmSolutionsModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSelectSolutionsController controller)
    {
        var result = (await controller.ConfirmSolutions(organisation.InternalIdentifier, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received(1).AcceptShortlist(organisation.InternalIdentifier, competitionId);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionsDashboardController.Index));
    }
}
