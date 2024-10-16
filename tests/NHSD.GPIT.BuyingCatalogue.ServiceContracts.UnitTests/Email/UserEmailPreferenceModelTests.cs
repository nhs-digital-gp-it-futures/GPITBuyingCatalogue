﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Email
{
    public static class UserEmailPreferenceModelTests
    {
        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Constructor(
            bool defaultPreference,
            EmailPreferenceTypeEnum emailPreferenceType,
            EmailPreferenceRoleType roleType)
        {
            var model = new UserEmailPreferenceModel(
                emailPreferenceType,
                defaultPreference,
                null,
                roleType);

            model.EmailPreferenceType.Should().Be(emailPreferenceType);
            model.DefaultEnabled.Should().Be(defaultPreference);
            model.UserEnabled.Should().BeNull();
            model.Enabled.Should().Be(defaultPreference);
            model.RoleType.Should().Be(roleType);
        }

        [Theory]
        [MockInlineAutoData(true, false)]
        [MockInlineAutoData(false, true)]
        public static void Constructor2(
            bool defaultPreference,
            bool userPreference,
            EmailPreferenceTypeEnum emailPreferenceType,
            EmailPreferenceRoleType roleType)
        {
            var model = new UserEmailPreferenceModel(
                EmailPreferenceTypeEnum.ContractExpiry,
                defaultPreference,
                userPreference,
                roleType);

            model.EmailPreferenceType.Should().Be(emailPreferenceType);
            model.DefaultEnabled.Should().Be(defaultPreference);
            model.UserEnabled.Should().Be(userPreference);
            model.Enabled.Should().Be(userPreference);
            model.RoleType.Should().Be(roleType);
        }
    }
}
