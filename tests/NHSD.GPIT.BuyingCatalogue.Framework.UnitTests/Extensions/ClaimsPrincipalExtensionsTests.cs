using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public static void GetPrimaryOrganisationName_NullPrincipal_ThrowsException()
        {
            ClaimsPrincipal user = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => user.GetPrimaryOrganisationName());
        }

        [Fact]
        public static void GetPrimaryOrganisationName_GetsValue()
        {
            var user = CreatePrincipal("primaryOrganisationName", "HULL CCJ");

            var result = user.GetPrimaryOrganisationName();

            Assert.Equal("HULL CCJ", result);
        }

        [Fact]
        public static void GetPrimaryOrganisationInternalIdentifier_NullPrincipal_ThrowsException()
        {
            ClaimsPrincipal user = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => user.GetPrimaryOrganisationInternalIdentifier());
        }

        [Fact]
        public static void GetPrimaryOrganisationInternalIdentifier_GetsValue()
        {
            var user = CreatePrincipal("primaryOrganisationInternalIdentifier", "3CY");

            var result = user.GetPrimaryOrganisationInternalIdentifier();

            Assert.Equal("3CY", result);
        }

        [Fact]
        public static void GetSecondaryOrganisationInternalIdentifiers_NullPrincipal_ThrowsException()
        {
            ClaimsPrincipal user = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => user.GetSecondaryOrganisationInternalIdentifiers());
        }

        [Fact]
        public static void GetSecondaryOrganisationInternalIdentifiers_GetsValues()
        {
            var user = CreatePrincipal("secondaryOrganisationInternalIdentifier", new[] { "3CY", "3BY", "ABC" });

            var result = user.GetSecondaryOrganisationInternalIdentifiers();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, s => s.EqualsIgnoreCase("3CY"));
            Assert.Contains(result, s => s.EqualsIgnoreCase("3BY"));
            Assert.Contains(result, s => s.EqualsIgnoreCase("ABC"));
        }

        [Fact]
        public static void GetUserDisplayName_NullPrincipal_ThrowsException()
        {
            ClaimsPrincipal user = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => user.GetUserDisplayName());
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
            var user = CreatePrincipal(ClaimTypes.Role, OrganisationFunction.Authority.Name);

            Assert.True(user.IsAdmin());
            Assert.False(user.IsBuyer());
            Assert.False(user.IsAccountManager());
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
            var user = CreatePrincipal(ClaimTypes.Role, OrganisationFunction.Buyer.Name);

            Assert.False(user.IsAdmin());
            Assert.False(user.IsAccountManager());
            Assert.True(user.IsBuyer());
        }

        [Fact]
        public static void IsBuyer_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            Assert.False(user.IsBuyer());
        }

        [Fact]
        public static void IsAccountManager_True_WithClaim()
        {
            var user = CreatePrincipal(ClaimTypes.Role, OrganisationFunction.AccountManager.Name);

            Assert.True(user.IsAccountManager());
            Assert.False(user.IsAdmin());
            Assert.False(user.IsBuyer());
        }

        [Fact]
        public static void IsAccountManager_False_WithoutClaim()
        {
            var user = CreatePrincipal("UnrelatedClaim", "True");

            Assert.False(user.IsAccountManager());
        }

        [Fact]
        public static void UserId_NullPrincipal_ThrowsException()
        {
            ClaimsPrincipal user = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => user.UserId());
        }

        [Fact]
        public static void UserId_ValidValueSet_ReturnsValidValue()
        {
            const int expected = 61;
            var user = CreatePrincipal("userId", expected.ToString());

            user.UserId().Should().Be(expected);
        }

        [Theory]
        [MockInlineAutoData(Permissions.ManageCatalogueSolutions, true)]
        [MockInlineAutoData(Permissions.ManageContractingVehicles, true)]
        [MockInlineAutoData(Permissions.ManageSupplierDefinedEpics, true)]
        [MockInlineAutoData(Permissions.ManageCapabilitiesAndEpics, true)]
        [MockInlineAutoData(Permissions.ManageInteroperability, true)]
        [MockInlineAutoData(Permissions.ManageBuyerOrganisations, false)]
        [MockInlineAutoData(Permissions.ManageSupplierOrganisations, false)]
        [MockInlineAutoData(Permissions.ManageUsers, false)]
        [MockInlineAutoData(Permissions.ManageAccountCreationRequests, false)]
        [MockInlineAutoData(Permissions.ManageAllowedEmailDomains, false)]
        [MockInlineAutoData(Permissions.ManageAllOrders, false)]
        public static void User_CanManageSolutions_ReturnsExpected(string claim, bool expected)
        {
            var user = CreatePrincipal(Permissions.ClaimType, claim);

            user.CanManageSolutions().Should().Be(expected);
        }

        [Theory]
        [MockInlineAutoData(Permissions.ManageCatalogueSolutions, false)]
        [MockInlineAutoData(Permissions.ManageContractingVehicles, false)]
        [MockInlineAutoData(Permissions.ManageSupplierDefinedEpics, false)]
        [MockInlineAutoData(Permissions.ManageCapabilitiesAndEpics, false)]
        [MockInlineAutoData(Permissions.ManageInteroperability, false)]
        [MockInlineAutoData(Permissions.ManageBuyerOrganisations, true)]
        [MockInlineAutoData(Permissions.ManageSupplierOrganisations, true)]
        [MockInlineAutoData(Permissions.ManageUsers, false)]
        [MockInlineAutoData(Permissions.ManageAccountCreationRequests, false)]
        [MockInlineAutoData(Permissions.ManageAllowedEmailDomains, false)]
        [MockInlineAutoData(Permissions.ManageAllOrders, false)]
        public static void User_CanManageOrganisations_ReturnsExpected(string claim, bool expected)
        {
            var user = CreatePrincipal(Permissions.ClaimType, claim);

            user.CanManageOrganisations().Should().Be(expected);
        }

        [Theory]
        [MockInlineAutoData(Permissions.ManageCatalogueSolutions, false)]
        [MockInlineAutoData(Permissions.ManageContractingVehicles, false)]
        [MockInlineAutoData(Permissions.ManageSupplierDefinedEpics, false)]
        [MockInlineAutoData(Permissions.ManageCapabilitiesAndEpics, false)]
        [MockInlineAutoData(Permissions.ManageInteroperability, false)]
        [MockInlineAutoData(Permissions.ManageBuyerOrganisations, false)]
        [MockInlineAutoData(Permissions.ManageSupplierOrganisations, false)]
        [MockInlineAutoData(Permissions.ManageUsers, true)]
        [MockInlineAutoData(Permissions.ManageAccountCreationRequests, true)]
        [MockInlineAutoData(Permissions.ManageAllowedEmailDomains, true)]
        [MockInlineAutoData(Permissions.ManageAllOrders, false)]
        public static void User_CanManageUsers_ReturnsExpected(string claim, bool expected)
        {
            var user = CreatePrincipal(Permissions.ClaimType, claim);

            user.CanManageUsers().Should().Be(expected);
        }

        [Theory]
        [MockInlineAutoData(Permissions.ManageCatalogueSolutions, false)]
        [MockInlineAutoData(Permissions.ManageContractingVehicles, false)]
        [MockInlineAutoData(Permissions.ManageSupplierDefinedEpics, false)]
        [MockInlineAutoData(Permissions.ManageCapabilitiesAndEpics, false)]
        [MockInlineAutoData(Permissions.ManageInteroperability, false)]
        [MockInlineAutoData(Permissions.ManageBuyerOrganisations, false)]
        [MockInlineAutoData(Permissions.ManageSupplierOrganisations, false)]
        [MockInlineAutoData(Permissions.ManageUsers, false)]
        [MockInlineAutoData(Permissions.ManageAccountCreationRequests, false)]
        [MockInlineAutoData(Permissions.ManageAllowedEmailDomains, false)]
        [MockInlineAutoData(Permissions.ManageAllOrders, true)]
        public static void User_CanManageOrders_ReturnsExpected(string claim, bool expected)
        {
            var user = CreatePrincipal(Permissions.ClaimType, claim);

            user.CanManageOrders().Should().Be(expected);
        }

        [Fact]
        public static void IsAdminUser_Returns_Expected()
        {
            var user = CreatePrincipal(ClaimTypes.Role, OrganisationFunction.Authority.Name);

            user.CanManageOrders().Should().Be(true);
            user.CanManageUsers().Should().Be(true);
            user.CanManageOrganisations().Should().Be(true);
            user.CanManageSolutions().Should().Be(true);
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, string value)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(claim, value) }, "mock"));
        }

        private static ClaimsPrincipal CreatePrincipal(string claim, IEnumerable<string> values)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(values.Select(s => new Claim(claim, s)), "mock"));
        }
    }
}
