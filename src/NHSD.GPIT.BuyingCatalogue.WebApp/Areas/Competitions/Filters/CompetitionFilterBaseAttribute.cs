using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;

public abstract class CompetitionFilterBaseAttribute : ActionFilterAttribute
{
    protected async Task<Competition> GetCompetitionFromRoute(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue(ParameterKeyConstants.InternalOrgIdKey, out var internalOrgId)
            || !context.ActionArguments.TryGetValue(ParameterKeyConstants.CompetitionIdKey, out var routeCompetitionId)
            || routeCompetitionId is null
            || !int.TryParse(routeCompetitionId.ToString(), out var competitionId))
        {
            return null;
        }

        var competitionsService = context.HttpContext.RequestServices.GetRequiredService<ICompetitionsService>();

        var competition = await competitionsService.GetCompetition(internalOrgId.ToString(), competitionId);

        return competition;
    }
}
