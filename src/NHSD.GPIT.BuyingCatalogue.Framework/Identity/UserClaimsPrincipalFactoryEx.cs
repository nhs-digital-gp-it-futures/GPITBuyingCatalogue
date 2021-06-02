using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public class UserClaimsPrincipalFactoryEx<TUser> : UserClaimsPrincipalFactory<TUser>
        where TUser : AspNetUser
    {
        private readonly ILogWrapper<UserClaimsPrincipalFactoryEx<TUser>> logger;
        private readonly IOrganisationsService organisationService;

        public UserClaimsPrincipalFactoryEx(
            UserManager<TUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            ILogWrapper<UserClaimsPrincipalFactoryEx<TUser>> logger,
            IOrganisationsService organisationService)
            : base(userManager, optionsAccessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var id = await base.GenerateClaimsAsync(user);

            var aspNetUser = (AspNetUser)user;

            id.AddClaim(new Claim("userDisplayName", $"{aspNetUser.FirstName} {aspNetUser.LastName}"));
            id.AddClaim(new Claim("organisationFunction", aspNetUser.OrganisationFunction));

            var organisation = await organisationService.GetOrganisation(aspNetUser.PrimaryOrganisationId);
            id.AddClaim(new Claim("primaryOrganisationOdsCode", organisation.OdsCode));

            var relatedOrganisations = await organisationService.GetRelatedOrganisations(aspNetUser.PrimaryOrganisationId);

            foreach (var relatedOrganisation in relatedOrganisations)
                id.AddClaim(new Claim("secondaryOrganisationOdsCode", relatedOrganisation.OdsCode));

            return id;
        }
    }
}
