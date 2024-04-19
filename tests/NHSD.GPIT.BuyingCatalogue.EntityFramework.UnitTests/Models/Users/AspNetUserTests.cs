using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Users
{
    public static class AspNetUserTests
    {
        [Theory]
        [MockAutoData]
        public static void HasAcceptedLatestTermsOfUse_Accepted_True(AspNetUser user)
        {
            var termsRevisionDate = DateTime.UtcNow.AddDays(-1);

            user.AcceptedTermsOfUseDate = DateTime.UtcNow;

            user.HasAcceptedLatestTermsOfUse(termsRevisionDate).Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void HasAcceptedLatestTermsOfUse_NewRevisionDate_False(AspNetUser user)
        {
            var termsRevisionDate = DateTime.UtcNow;

            user.AcceptedTermsOfUseDate = termsRevisionDate.AddDays(-5);

            user.HasAcceptedLatestTermsOfUse(termsRevisionDate).Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void HasAcceptedLatestTermsOfUse_NotAccepted_False(AspNetUser user)
        {
            var termsRevisionDate = DateTime.UtcNow;

            user.AcceptedTermsOfUseDate = null;

            user.HasAcceptedLatestTermsOfUse(termsRevisionDate).Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void RemainingPasswordExpiryDays_ReturnsExpected(
            AspNetUser user)
        {
            const int dateDifference = 14;

            var today = new DateTime(2024, 4, 16);
            var changedDate = today.AddYears(-1).AddDays(dateDifference);

            user.PasswordUpdatedDate = changedDate;

            user.RemainingPasswordExpiryDays(today).Should().Be(dateDifference);
        }

        [Theory]
        [MockAutoData]
        public static void RemainingPasswordExpiryDays_DateExceedsYear_ReturnsExpected(
            AspNetUser user)
        {
            var today = new DateTime(2024, 4, 16);
            var changedDate = today.AddYears(-2);

            user.PasswordUpdatedDate = changedDate;

            user.RemainingPasswordExpiryDays(today).Should().Be(0);
        }

        [Theory]
        [MockAutoData]
        public static void DetermineEventToRaise_ZeroDaysRemaining_ReturnsExpected(
            AspNetUser user)
        {
            var today = new DateTime(2024, 4, 16);
            var changedDate = today.AddYears(-2);

            user.Events.Clear();
            user.PasswordUpdatedDate = changedDate;

            user.DetermineEventToRaise(today).Should().Be(PasswordExpiryEventTypeEnum.Nothing);
        }

        [Theory]
        [MockInlineAutoData(30, EventTypeEnum.PasswordEnteredFirstExpiryThreshold)]
        [MockInlineAutoData(14, EventTypeEnum.PasswordEnteredSecondExpiryThreshold)]
        [MockInlineAutoData(1, EventTypeEnum.PasswordEnteredThirdExpiryThreshold)]
        public static void DetermineEventToRaise_ReturnsExpected(
            int days,
            PasswordExpiryEventTypeEnum expected,
            AspNetUser user)
        {
            var today = new DateTime(2024, 4, 16);
            var changedDate = today.AddYears(-1).AddDays(days);

            user.Events.Clear();
            user.PasswordUpdatedDate = changedDate;

            user.DetermineEventToRaise(today).Should().Be(expected);
        }

        [Theory]
        [MockInlineAutoData(30, EventTypeEnum.PasswordEnteredFirstExpiryThreshold)]
        [MockInlineAutoData(14, EventTypeEnum.PasswordEnteredSecondExpiryThreshold)]
        [MockInlineAutoData(1, EventTypeEnum.PasswordEnteredThirdExpiryThreshold)]
        public static void DetermineEventToRaise_ExistingEvent_ReturnsExpected(
            int days,
            PasswordExpiryEventTypeEnum expected,
            AspNetUser user)
        {
            var today = new DateTime(2024, 4, 16);
            var changedDate = today.AddYears(-1).AddDays(days);

            user.Events = new List<AspNetUserEvent> { new AspNetUserEvent((int)expected) };
            user.PasswordUpdatedDate = changedDate;

            user.DetermineEventToRaise(today).Should().Be(PasswordExpiryEventTypeEnum.Nothing);
        }
    }
}
