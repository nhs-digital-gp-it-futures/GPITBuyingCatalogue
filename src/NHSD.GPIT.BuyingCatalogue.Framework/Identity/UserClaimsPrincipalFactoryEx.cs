using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public sealed class UserClaimsPrincipalFactoryEx : UserClaimsPrincipalFactory<AspNetUser>
    {
        private readonly IOrganisationsService organisationService;

        public UserClaimsPrincipalFactoryEx(
            UserManager<AspNetUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            IOrganisationsService organisationService)
            : base(userManager, optionsAccessor)
        {
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AspNetUser user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var id = await base.GenerateClaimsAsync(user);

            id.AddClaim(new Claim(Constants.Claims.UserDisplayName, $"{user.FirstName} {user.LastName}"));
            id.AddClaim(new Claim(Constants.Claims.UserId, user.Id.ToString()));
            id.AddClaim(new Claim(Constants.Claims.OrganisationFunction, user.OrganisationFunction));

            var organisation = await organisationService.GetOrganisation(user.PrimaryOrganisationId);
            id.AddClaim(new Claim(Constants.Claims.PrimaryOrganisationOdsCode, organisation.OdsCode));

            var relatedOrganisations = await organisationService.GetRelatedOrganisations(user.PrimaryOrganisationId);

            foreach (var relatedOrganisation in relatedOrganisations)
                id.AddClaim(new Claim(Constants.Claims.SecondaryOrganisationOdsCode, relatedOrganisation.OdsCode));

            return id;
        }
    }
}
