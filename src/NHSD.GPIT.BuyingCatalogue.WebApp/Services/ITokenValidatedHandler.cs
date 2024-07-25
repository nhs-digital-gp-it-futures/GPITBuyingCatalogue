using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Services;

public interface ITokenValidatedHandler
{
    Task HandleAsync(TokenValidatedContext context, AspNetUser user);
}
