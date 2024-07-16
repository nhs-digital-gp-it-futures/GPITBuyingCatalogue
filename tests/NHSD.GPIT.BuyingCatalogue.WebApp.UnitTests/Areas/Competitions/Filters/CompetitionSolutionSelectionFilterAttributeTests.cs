using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Filters;

public static class CompetitionSolutionSelectionFilterAttributeTests
{
    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_NoCompetitionId_Returns(
        string organisationId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CompetitionSolutionSelectionFilterAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisationId);

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_NoOrganisationId_Returns(
        int competitionId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CompetitionSolutionSelectionFilterAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competitionId.ToString());

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_InvalidCompetitionIdFormat_Returns(
        string organisationId,
        string competitionId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CompetitionSolutionSelectionFilterAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisationId);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competitionId);

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_ShortlistAccepted_SetsResult(
        Organisation organisation,
        Competition competition,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSolutionSelectionFilterAttribute filter)
    {
        competition.Organisation = organisation;
        competition.Completed = null;
        competition.ShortlistAccepted = DateTime.UtcNow;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisation.InternalIdentifier);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competition.Id.ToString());

        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(competitionsService)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<RedirectToActionResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_CompetitionCompleted_SetsResult(
        Organisation organisation,
        Competition competition,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSolutionSelectionFilterAttribute filter)
    {
        competition.Organisation = organisation;
        competition.Completed = DateTime.UtcNow;
        competition.ShortlistAccepted = null;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisation.InternalIdentifier);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competition.Id.ToString());

        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(competitionsService)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<RedirectToActionResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_CompetitionInProgress_Returns(
        Organisation organisation,
        Competition competition,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionSolutionSelectionFilterAttribute filter)
    {
        competition.Completed = null;
        competition.ShortlistAccepted = null;
        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisation.InternalIdentifier);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competition.Id.ToString());

        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(competitionsService)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }
}
