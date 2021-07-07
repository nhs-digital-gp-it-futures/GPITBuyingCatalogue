using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public class UserClaimsPrincipalFactoryEx<TUser> : UserClaimsPrincipalFactory<TUser>
        where TUser : AspNetUser
    {
        private readonly IOrganisationsService organisationService;

        public UserClaimsPrincipalFactoryEx(
            UserManager<TUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            IOrganisationsService organisationService)
            : base(userManager, optionsAccessor)
        {
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var id = await base.GenerateClaimsAsync(user);

            var aspNetUser = (AspNetUser)user;

            id.AddClaim(new Claim(Constants.Claims.UserDisplayName, $"{aspNetUser.FirstName} {aspNetUser.LastName}"));
            id.AddClaim(new Claim(Constants.Claims.UserId, aspNetUser.Id));
            id.AddClaim(new Claim(Constants.Claims.OrganisationFunction, aspNetUser.OrganisationFunction));

            var organisation = await organisationService.GetOrganisation(aspNetUser.PrimaryOrganisationId);
            id.AddClaim(new Claim(Constants.Claims.PrimaryOrganisationOdsCode, organisation.OdsCode));

            var relatedOrganisations = await organisationService.GetRelatedOrganisations(aspNetUser.PrimaryOrganisationId);

            foreach (var relatedOrganisation in relatedOrganisations)
                id.AddClaim(new Claim(Constants.Claims.SecondaryOrganisationOdsCode, relatedOrganisation.OdsCode));

            return id;
        }
    }
}
