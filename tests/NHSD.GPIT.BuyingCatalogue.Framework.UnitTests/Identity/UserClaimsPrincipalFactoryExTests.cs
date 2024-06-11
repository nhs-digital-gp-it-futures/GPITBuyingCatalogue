using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity
{
    public static class UserClaimsPrincipalFactoryExTests
    {
        [Theory]
        [MockInlineAutoData("Buyer")]
        [MockInlineAutoData("Authority")]
        [MockInlineAutoData("AccountManager")]
        public static async Task GenerateClaimsAsync_ClaimsSetBasedOnAuthorityAndFirstLastName(
            string organisationFunction,
            [Frozen] IOrganisationsService orgService,
            [Frozen] IOptions<IdentityOptions> options,
            [Frozen] IUserRoleStore<AspNetUser> store)
        {
            var user = new AspNetUser { Id = 97, UserName = "Foo", FirstName = "Fred", LastName = "Smith" };
            var userManager = MockUserManager(store);

            userManager.GetRolesAsync(user).Returns(new List<string> { organisationFunction });

            orgService.GetOrganisation(Arg.Any<int>()).Returns(new Organisation { InternalIdentifier = "CG-123" });

            orgService.GetRelatedOrganisations(Arg.Any<int>()).Returns(new List<Organisation>
            {
                new() { InternalIdentifier = "CG-ABC" },
                new() { InternalIdentifier = "CG-DEF" },
                new() { InternalIdentifier = "CG-GHI" },
            });

            var identityOptions = new IdentityOptions();
            options.Value.Returns(identityOptions);

            userManager.Options = identityOptions;

            var factory = new CatalogueUserClaimsPrincipalFactory(
                userManager,
                options,
                orgService);

            var principal = await factory.CreateAsync(user);

            Assert.Equal(organisationFunction, GetClaimValue(principal, ClaimTypes.Role));
            Assert.Equal("Fred Smith", GetClaimValue(principal, "userDisplayName"));
            Assert.Equal("CG-123", GetClaimValue(principal, "primaryOrganisationInternalIdentifier"));
            Assert.Equal(3, GetClaimValues(principal, "secondaryOrganisationInternalIdentifier").Length);

            Assert.Contains(GetClaimValues(principal, "secondaryOrganisationInternalIdentifier"), s => s.EqualsIgnoreCase("CG-ABC"));
            Assert.Contains(GetClaimValues(principal, "secondaryOrganisationInternalIdentifier"), s => s.EqualsIgnoreCase("CG-DEF"));
            Assert.Contains(GetClaimValues(principal, "secondaryOrganisationInternalIdentifier"), s => s.EqualsIgnoreCase("CG-GHI"));
        }

        public static UserManager<AspNetUser> MockUserManager(IUserStore<AspNetUser> store)
        {
            //var store = new IUserStore<AspNetUser>();
            var mgr = new UserManager<AspNetUser>(store, null, null, null, null, null, null, null, null);
            mgr.UserValidators.Add(new UserValidator<AspNetUser>());
            mgr.PasswordValidators.Add(new PasswordValidator<AspNetUser>());
            return mgr;
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase));

            return claim is not null ? claim.Value : string.Empty;
        }

        private static string[] GetClaimValues(ClaimsPrincipal user, string claimType)
        {
            return user.Claims.Where(c => c.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase)).Select(c => c.Value).ToArray();
        }
    }
}
