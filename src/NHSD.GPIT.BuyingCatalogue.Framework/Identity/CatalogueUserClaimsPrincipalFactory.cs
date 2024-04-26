using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public sealed class CatalogueUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AspNetUser>
    {
        private readonly IOrganisationsService organisationService;
        private readonly UserManager<AspNetUser> userManager;

        public CatalogueUserClaimsPrincipalFactory(
            UserManager<AspNetUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            IOrganisationsService organisationService)
            : base(userManager, optionsAccessor)
        {
            this.userManager = userManager;
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AspNetUser user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var id = await base.GenerateClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            id.AddClaim(new Claim(Constants.CatalogueClaims.UserDisplayName, $"{user.FirstName} {user.LastName}"));
            id.AddClaim(new Claim(Constants.CatalogueClaims.UserId, user.Id.ToString(CultureInfo.InvariantCulture)));

            foreach (var role in roles)
            {
                id.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var organisation = await organisationService.GetOrganisation(user.PrimaryOrganisationId);
            id.AddClaim(new Claim(Constants.CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisation.InternalIdentifier));

            var relatedOrganisations = await organisationService.GetRelatedOrganisations(user.PrimaryOrganisationId);

            foreach (var relatedOrganisation in relatedOrganisations)
                id.AddClaim(new Claim(Constants.CatalogueClaims.SecondaryOrganisationInternalIdentifier, relatedOrganisation.InternalIdentifier));

            return id;
        }
    }
}
