using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity
{
    public static class UserClaimsPrincipalFactoryExTests
    {
        [Theory]
        [InlineData("Buyer")]
        [InlineData("Authority")]
        public static async Task GenerateClaimsAsync_ClaimsSetBasedOnAuthorityAndFirstLastName(string organisationFunction)
        {
            var user = new AspNetUser { Id = Guid.NewGuid(), UserName = "Foo", OrganisationFunction = organisationFunction, FirstName = "Fred", LastName = "Smith" };
            var userManager = MockUserManager();
            userManager.Setup(m => m.GetUserIdAsync(user)).ReturnsAsync(user.Id.ToString());
            userManager.Setup(m => m.GetUserNameAsync(user)).ReturnsAsync(user.UserName);

            var orgService = new Mock<IOrganisationsService>();
            orgService.Setup(m => m.GetOrganisation(It.IsAny<int>())).ReturnsAsync(new Organisation { OdsCode = "123" });

            orgService.Setup(m => m.GetRelatedOrganisations(It.IsAny<int>())).ReturnsAsync(new List<Organisation>
            {
                new() { OdsCode = "ABC" },
                new() { OdsCode = "DEF" },
                new() { OdsCode = "GHI" },
            });

            var options = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions();
            options.Setup(a => a.Value).Returns(identityOptions);

            userManager.Object.Options = identityOptions;

            var factory = new UserClaimsPrincipalFactoryEx(
                userManager.Object,
                options.Object,
                orgService.Object);

            var principal = await factory.CreateAsync(user);

            Assert.Equal(organisationFunction, GetClaimValue(principal, "organisationFunction"));
            Assert.Equal("Fred Smith", GetClaimValue(principal, "userDisplayName"));
            Assert.Equal("123", GetClaimValue(principal, "primaryOrganisationOdsCode"));
            Assert.Equal(3, GetClaimValues(principal, "secondaryOrganisationOdsCode").Length);

            Assert.Contains(GetClaimValues(principal, "secondaryOrganisationOdsCode"), s => s.EqualsIgnoreCase("ABC"));
            Assert.Contains(GetClaimValues(principal, "secondaryOrganisationOdsCode"), s => s.EqualsIgnoreCase("DEF"));
            Assert.Contains(GetClaimValues(principal, "secondaryOrganisationOdsCode"), s => s.EqualsIgnoreCase("GHI"));
        }

        public static Mock<UserManager<AspNetUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<AspNetUser>>();
            var mgr = new Mock<UserManager<AspNetUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<AspNetUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<AspNetUser>());
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
