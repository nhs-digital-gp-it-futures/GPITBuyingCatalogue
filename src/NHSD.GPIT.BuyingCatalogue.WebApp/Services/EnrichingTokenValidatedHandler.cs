using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Services;

/// <summary>
/// Enriches the <see cref="ClaimsPrincipal"/> to add local claims from the users database.
/// </summary>
/// <param name="organisationsService">An <see cref="IOrganisationsService"/> used for retrieving organisation details.</param>
public class EnrichingTokenValidatedHandler(IOrganisationsService organisationsService) : ITokenValidatedHandler
{
    public async Task HandleAsync(TokenValidatedContext context, AspNetUser user)
    {
        var organisation = await organisationsService.GetOrganisation(user.PrimaryOrganisationId);
        var secondaryOrganisations =
            await organisationsService.GetRelatedOrganisations(organisation.Id);

        var userRoles = user.AspNetUserRoles.Select(x => x.Role.Name);

        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim(CatalogueClaims.UserId, user.Id.ToString(CultureInfo.InvariantCulture)));
        claimsIdentity.AddClaim(
            new Claim(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisation.InternalIdentifier));

        claimsIdentity.AddClaims(userRoles.Select(x => new Claim(ClaimTypes.Role, x!)));
        claimsIdentity.AddClaims(
            secondaryOrganisations.Select(
                x => new Claim(
                    CatalogueClaims.SecondaryOrganisationInternalIdentifier,
                    x.InternalIdentifier)));

        context.Principal!.AddIdentity(claimsIdentity);
    }
}
