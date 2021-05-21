using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class UserClaimsPrincipalFactoryExTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            var options = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions();
            options.Setup(a => a.Value).Returns(identityOptions);

            var ex = Assert.Throws<ArgumentNullException>(() =>
                _ = new UserClaimsPrincipalFactoryEx<AspNetUser>(
                MockUserManager<AspNetUser>().Object,
                options.Object,
                null,
                Mock.Of<IOrganisationsService>()));

            Assert.AreEqual("Value cannot be null. (Parameter 'logger')", ex.Message);
        }

        [Test]
        [TestCase("Buyer")]
        [TestCase("Authority")]
        public static async Task GenerateClaimsAsync_ClaimsSetBasedOnAuthorityAndFirstLastName(string organisationFunction)
        {
            var user = new AspNetUser { Id = "123", UserName = "Foo", OrganisationFunction = organisationFunction, FirstName = "Fred", LastName = "Smith" };
            var userManager = MockUserManager<AspNetUser>();
            userManager.Setup(m => m.GetUserIdAsync(user)).ReturnsAsync(user.Id);
            userManager.Setup(m => m.GetUserNameAsync(user)).ReturnsAsync(user.UserName);

            var options = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions();
            options.Setup(a => a.Value).Returns(identityOptions);

            userManager.Object.Options = identityOptions;

            var factory = new UserClaimsPrincipalFactoryEx<AspNetUser>(
                userManager.Object,
                options.Object,
                Mock.Of<ILogWrapper<UserClaimsPrincipalFactoryEx<AspNetUser>>>(),
                Mock.Of<IOrganisationsService>());

            var principal = await factory.CreateAsync(user);

            Assert.AreEqual(organisationFunction, GetClaimValue(principal, "organisationFunction"));
            Assert.AreEqual("Fred Smith", GetClaimValue(principal, "userDisplayName"));
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase));

            return claim != null ? claim.Value : string.Empty;
        }
    }
}
