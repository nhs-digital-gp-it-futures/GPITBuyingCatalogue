using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClaimsPrincipalExtensionsTests
    {
        [Test]
        public static void GetPrimaryOrganisationName_GetsValue ()
        {
            var user = CreatePrincipal("primaryOrganisationName", "HULL CCJ");

            var result = user.GetPrimaryOrganisationName();

            Assert.AreEqual("HULL CCJ", result);
        }

        [Test]
        public static void GetUserDisplayName_GetsValue()
        {
            var user = CreatePrincipal("userDisplayName", "Bill Smith");

            var result = user.GetUserDisplayName();

            Assert.AreEqual("Bill Smith", result);
        }

        [Test]
        public static void IsAdmin_True_WithClaim()
        {
            var user = CreatePrincipal("IsAdmin", "True");

            var result = user.IsAdmin();

            Assert.True(result);
        }

        [Test]
        public static void IsAdmin_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            var result = user.IsAdmin();

            Assert.False(result);
        }

        [Test]
        public static void IsBuyer_True_WithClaim()
        {
            var user = CreatePrincipal("IsBuyer", "True");

            var result = user.IsBuyer();

            Assert.True(result);
        }

        [Test]
        public static void IsBuyer_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            var result = user.IsBuyer();

            Assert.False(result);
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, string value)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                new Claim(claim, value),
            }, "mock"));
        }
    }
}
