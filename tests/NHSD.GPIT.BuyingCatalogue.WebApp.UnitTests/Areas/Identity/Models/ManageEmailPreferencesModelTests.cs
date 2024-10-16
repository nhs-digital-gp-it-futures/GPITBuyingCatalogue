﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class ManageEmailPreferencesModelTests
    {
        [Theory]
        [MockInlineAutoData(EmailPreferenceTypeEnum.ContractExpiry, ManageEmailPreferencesModel.ContractExpiryLabel)]
        [MockInlineAutoData(EmailPreferenceTypeEnum.PasswordExpiry, ManageEmailPreferencesModel.PasswordExpiryLabel)]
        public static void GetLabel(
            EmailPreferenceTypeEnum emailPreferenceTypeEnum,
            string expectedValue,
            ManageEmailPreferencesModel model)
        {
            model.GetLabel(emailPreferenceTypeEnum).Should().Be(expectedValue);
        }

        [Theory]
        [MockInlineAutoData(EmailPreferenceTypeEnum.ContractExpiry, ManageEmailPreferencesModel.ContractExpiryHint)]
        [MockInlineAutoData(EmailPreferenceTypeEnum.PasswordExpiry, ManageEmailPreferencesModel.PasswordExpiryHint)]
        public static void GetHint(
            EmailPreferenceTypeEnum emailPreferenceTypeEnum,
            string expectedValue,
            ManageEmailPreferencesModel model)
        {
            model.GetHint(emailPreferenceTypeEnum).Should().Be(expectedValue);
        }
    }
}
