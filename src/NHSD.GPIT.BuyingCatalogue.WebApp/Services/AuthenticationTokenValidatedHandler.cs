using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Services;

/// <summary>
/// Performs a second authentication attempt to ensure the user is allowed to access the system.
/// Sets the <see cref="TokenValidatedContext"/> Result if the user is unauthorised.
/// </summary>
public class AuthenticationTokenValidatedHandler : ITokenValidatedHandler
{
    public async Task HandleAsync(TokenValidatedContext context, AspNetUser user)
    {
        if (user is not null) return;

        await context.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        context.Response.Redirect("/unauthorized");
        context.HandleResponse();
    }
}
