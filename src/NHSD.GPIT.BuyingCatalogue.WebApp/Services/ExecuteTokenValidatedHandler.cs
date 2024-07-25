using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Services;

public class ExecuteTokenValidatedHandler(
    IEnumerable<ITokenValidatedHandler> handlers,
    BuyingCatalogueDbContext dbContext) : IExecuteTokenValidatedHandler
{
    public async Task ExecuteAsync(TokenValidatedContext context)
    {
        var emailClaim = context.SecurityToken.Claims.FirstOrDefault(x => x.Type == CatalogueClaims.Email);
        if (emailClaim == null || string.IsNullOrWhiteSpace(emailClaim.Value)) return;

        var user = await dbContext.AspNetUsers.Include(aspNetUser => aspNetUser.AspNetUserRoles)
            .ThenInclude(aspNetUserRole => aspNetUserRole.Role)
            .FirstOrDefaultAsync(
                x => x.Email == emailClaim.Value);

        foreach (var handler in handlers)
        {
            if (context.Result is { Handled: true })
                break;

            await handler.HandleAsync(context, user);
        }
    }
}
