using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Filters;

public static class CriteriaReviewedGuardFilterAttributeTests
{
    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_NoCompetitionId_Returns(
        string organisationId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CriteriaReviewedGuardFilterAttribute filter)
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
        CriteriaReviewedGuardFilterAttribute filter)
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
        CriteriaReviewedGuardFilterAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisationId);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competitionId);

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_CompetitionCriteriaReviewed_SetsResult(
        Organisation organisation,
        Competition competition,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] ICompetitionsService competitionsService,
        CriteriaReviewedGuardFilterAttribute filter)
    {
        competition.Organisation = organisation;
        competition.HasReviewedCriteria = true;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisation.InternalIdentifier);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competition.Id.ToString());

        context.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(competitionsService)
            .BuildServiceProvider();

        await filter.OnActionExecutionAsync(context, next);

        var result = context.Result.As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionTaskListController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionTaskListController).ControllerName());
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecution_CompetitionCriteriaNotReviewed_Returns(
        Organisation organisation,
        Competition competition,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] ICompetitionsService competitionsService,
        CriteriaReviewedGuardFilterAttribute filter)
    {
        competition.HasReviewedCriteria = false;

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
