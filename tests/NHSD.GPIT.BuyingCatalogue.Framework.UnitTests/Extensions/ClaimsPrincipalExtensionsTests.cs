using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public static void GetPrimaryOrganisationName_GetsValue()
        {
            var user = CreatePrincipal("primaryOrganisationName", "HULL CCJ");

            var result = user.GetPrimaryOrganisationName();

            Assert.Equal("HULL CCJ", result);
        }

        [Fact]
        public static void GetPrimaryOdsCode_GetsValue()
        {
            var user = CreatePrincipal("primaryOrganisationOdsCode", "3CY");

            var result = user.GetPrimaryOdsCode();

            Assert.Equal("3CY", result);
        }

        [Fact]
        public static void GetSecondaryOdsCode_GetsValues()
        {
            var user = CreatePrincipal("secondaryOrganisationOdsCode", new[] { "3CY", "3BY", "ABC" });

            var result = user.GetSecondaryOdsCodes();

            Assert.Equal(3, result.Length);
            Assert.Contains(result, s => s.EqualsIgnoreCase("3CY"));
            Assert.Contains(result, s => s.EqualsIgnoreCase("3BY"));
            Assert.Contains(result, s => s.EqualsIgnoreCase("ABC"));
        }

        [Fact]
        public static void GetUserDisplayName_GetsValue()
        {
            var user = CreatePrincipal("userDisplayName", "Bill Smith");

            Assert.Equal("Bill Smith", user.GetUserDisplayName());
        }

        [Fact]
        public static void IsAdmin_True_WithClaim()
        {
            var user = CreatePrincipal("organisationFunction", "Authority");

            Assert.True(user.IsAdmin());
            Assert.False(user.IsBuyer());
        }

        [Fact]
        public static void IsAdmin_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            Assert.False(user.IsAdmin());
        }

        [Fact]
        public static void IsBuyer_True_WithClaim()
        {
            var user = CreatePrincipal("organisationFunction", "Buyer");

            Assert.False(user.IsAdmin());
            Assert.True(user.IsBuyer());
        }

        [Fact]
        public static void IsBuyer_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            Assert.False(user.IsBuyer());
        }

        [Fact]
        public static void UserId_ValidValueSet_ReturnsValidValue()
        {
            var expected = Guid.NewGuid();
            var user = CreatePrincipal("userId", expected.ToString());

            user.UserId().Should().Be(expected);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("")]
        [InlineData("   ")]
        public static void UserId_InvalidValue_ReturnsEmptyGuid(string invalid)
        {
            var user = CreatePrincipal("userId", invalid);

            user.UserId().Should().Be(Guid.Empty);
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, string value)
        {
            return new(new ClaimsIdentity(new Claim[] { new(claim, value) }, "mock"));
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, IEnumerable<string> values)
        {
            return new(new ClaimsIdentity(values.Select(s => new Claim(claim, s)), "mock"));
        }
    }
}
