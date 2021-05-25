﻿using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClaimsPrincipalExtensionsTests
    {
        [Test]
        public static void GetPrimaryOrganisationName_GetsValue()
        {
            var user = CreatePrincipal("primaryOrganisationName", "HULL CCJ");

            var result = user.GetPrimaryOrganisationName();

            Assert.AreEqual("HULL CCJ", result);
        }

        [Test]
        public static void GetPrimaryOdsCode_GetsValue()
        {
            var user = CreatePrincipal("primaryOrganisationOdsCode", "3CY");

            var result = user.GetPrimaryOdsCode();

            Assert.AreEqual("3CY", result);
        }

        [Test]
        public static void GetSecondaryOdsCode_GetsValues()
        {
            var user = CreatePrincipal("secondaryOrganisationOdsCode", new[] { "3CY", "3BY", "ABC" });

            var result = user.GetSecondaryOdsCodes();

            Assert.AreEqual(3, result.Length);
            Assert.True(result.Any(x => x.EqualsIgnoreCase("3CY")));
            Assert.True(result.Any(x => x.EqualsIgnoreCase("3BY")));
            Assert.True(result.Any(x => x.EqualsIgnoreCase("ABC")));
        }

        [Test]
        public static void GetUserDisplayName_GetsValue()
        {
            var user = CreatePrincipal("userDisplayName", "Bill Smith");

            Assert.AreEqual("Bill Smith", user.GetUserDisplayName());
        }

        [Test]
        public static void IsAdmin_True_WithClaim()
        {
            var user = CreatePrincipal("organisationFunction", "Authority");

            Assert.True(user.IsAdmin());
            Assert.False(user.IsBuyer());
        }

        [Test]
        public static void IsAdmin_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            Assert.False(user.IsAdmin());
        }

        [Test]
        public static void IsBuyer_True_WithClaim()
        {
            var user = CreatePrincipal("organisationFunction", "Buyer");

            Assert.False(user.IsAdmin());
            Assert.True(user.IsBuyer());
        }

        [Test]
        public static void IsBuyer_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            Assert.False(user.IsBuyer());
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, string value)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                new Claim(claim, value),
            }, "mock"));
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, string[] values)
        {            
            return new ClaimsPrincipal(new ClaimsIdentity(values.Select(x => new Claim(claim, x)), "mock"));
        }
    }
}
