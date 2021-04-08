using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public class UserClaimsPrincipalFactoryEx<TUser> : UserClaimsPrincipalFactory<TUser> where TUser : AspNetUser
    {
        private readonly ILogger<UserClaimsPrincipalFactoryEx<TUser>> _logger;

        public UserClaimsPrincipalFactoryEx(
            UserManager<TUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<UserClaimsPrincipalFactoryEx<TUser>> logger) : base(userManager, optionsAccessor)
        {
            _logger = logger;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var id = await base.GenerateClaimsAsync(user);

            var aspNetUser = (AspNetUser)user;

            id.AddClaim(new Claim("userDisplayName", $"{aspNetUser.FirstName} {aspNetUser.LastName}"));

            if( aspNetUser.OrganisationFunction.Equals(OrganisationFunction.Authority.DisplayName, StringComparison.InvariantCultureIgnoreCase))
                id.AddClaim(new Claim("IsAdmin", "True"));

            if (aspNetUser.OrganisationFunction.Equals(OrganisationFunction.Buyer.DisplayName, StringComparison.InvariantCultureIgnoreCase))
                id.AddClaim(new Claim("IsBuyer", "True"));

            return id;
        }
    }
}
