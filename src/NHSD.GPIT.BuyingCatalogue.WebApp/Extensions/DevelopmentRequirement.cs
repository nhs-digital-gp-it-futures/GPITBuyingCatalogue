using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NHSD.GPIT.BuyingCatalogue.Framework.Environments;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;

public sealed class DevelopmentRequirement : AuthorizationHandler<DevelopmentRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DevelopmentRequirement requirement)
    {
        if (CurrentEnvironment.IsDevelopment)
        {
            context.Succeed(requirement);
        }

        return Task.FromResult(0);
    }
}
