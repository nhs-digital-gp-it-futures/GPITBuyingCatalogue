using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Filters;

public static class CompetitionFilterBaseAttributeTests
{
    [Theory]
    [CommonAutoData]
    public static async Task OnActionExecution_NoCompetitionId_Returns(
        string organisationId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CompetitionFilterBaseAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisationId);

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task OnActionExecution_NoOrganisationId_Returns(
        int competitionId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CompetitionFilterBaseAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competitionId.ToString());

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task OnActionExecution_InvalidCompetitionIdFormat_Returns(
        string organisationId,
        string competitionId,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        CompetitionFilterBaseAttribute filter)
    {
        context.ActionArguments.Clear();
        context.ActionArguments.Add(ParameterKeyConstants.InternalOrgIdKey, organisationId);
        context.ActionArguments.Add(ParameterKeyConstants.CompetitionIdKey, competitionId);

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }
}
