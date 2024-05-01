using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email;

public static class EmailPreferenceRoleTypeExtensionsTests
{
    public static IEnumerable<object[]> IsRoleMatchTestData => new[]
    {
        new object[]
        {
            EmailPreferenceRoleType.All, new AspNetRole { Name = OrganisationFunction.Authority.Name }, true,
        },
        new object[]
        {
            EmailPreferenceRoleType.All, new AspNetRole { Name = OrganisationFunction.Buyer.Name }, true,
        },
        new object[]
        {
            EmailPreferenceRoleType.All,
            new AspNetRole { Name = OrganisationFunction.AccountManager.Name },
            true,
        },
        new object[]
        {
            EmailPreferenceRoleType.Buyers,
            new AspNetRole { Name = OrganisationFunction.Authority.Name },
            false,
        },
        new object[]
        {
            EmailPreferenceRoleType.Buyers, new AspNetRole { Name = OrganisationFunction.Buyer.Name }, true,
        },
        new object[]
        {
            EmailPreferenceRoleType.Buyers,
            new AspNetRole { Name = OrganisationFunction.AccountManager.Name },
            true,
        },
        new object[]
        {
            EmailPreferenceRoleType.Admins,
            new AspNetRole { Name = OrganisationFunction.Authority.Name },
            true,
        },
        new object[]
        {
            EmailPreferenceRoleType.Admins, new AspNetRole { Name = OrganisationFunction.Buyer.Name }, false,
        },
        new object[]
        {
            EmailPreferenceRoleType.Admins,
            new AspNetRole { Name = OrganisationFunction.AccountManager.Name },
            false,
        },
    };

    [Theory]
    [MockMemberAutoData(nameof(IsRoleMatchTestData))]
    public static void IsRoleMatch_Expected(
        EmailPreferenceRoleType emailPreferenceRoleType,
        AspNetRole role,
        bool expected)
    {
        emailPreferenceRoleType.IsRoleMatch(new List<AspNetRole> { role }).Should().Be(expected);
    }

    [Theory]
    [MockAutoData]
    public static void IsRoleMatch_InvalidRoleType_ThrowsOutOfRangeException(
        AspNetRole role) => FluentActions
        .Invoking(() => EmailPreferenceRoleType.None.IsRoleMatch(new List<AspNetRole> { role }))
        .Should()
        .Throw<ArgumentOutOfRangeException>();
}
