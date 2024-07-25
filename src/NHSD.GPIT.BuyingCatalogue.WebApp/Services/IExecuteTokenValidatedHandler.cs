using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Services;

public interface IExecuteTokenValidatedHandler
{
    Task ExecuteAsync(TokenValidatedContext context);
}
