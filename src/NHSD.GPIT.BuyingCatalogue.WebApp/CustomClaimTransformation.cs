using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp;

public class CustomClaimTransformation(
    BuyingCatalogueDbContext dbContext,
    IOrganisationsService organisationsService) : IClaimsTransformation
{
    private readonly BuyingCatalogueDbContext dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly IOrganisationsService organisationsService =
        organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == "email");
        if (emailClaim is null || string.IsNullOrWhiteSpace(emailClaim.Value))
            return principal;

        var user = await dbContext.AspNetUsers.Include(x => x.AspNetUserRoles)
            .ThenInclude(y => y.Role)
            .FirstOrDefaultAsync(x => x.Email == emailClaim.Value);

        if (user is null) return principal;

        var organisation = await organisationsService.GetOrganisation(user.PrimaryOrganisationId);

        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)));
        claimsIdentity.AddClaim(new Claim("userDisplayName", $"{user.FirstName} {user.LastName}"));
        claimsIdentity.AddClaim(new Claim("userId", user.Id.ToString(CultureInfo.InvariantCulture)));
        claimsIdentity.AddClaim(new Claim("primaryOrganisationInternalIdentifier", organisation.InternalIdentifier));

        var userRoles = user.AspNetUserRoles.Select(x => x.Role.Name);
        foreach (var role in userRoles)
        {
            claimsIdentity.AddClaim(new(ClaimTypes.Role, role!));
        }

        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}
