using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Users
{
    public static class AspNetUserTests
    {
        [Theory]
        [CommonAutoData]
        public static void HasAcceptedLatestTermsOfUse_Accepted_True(AspNetUser user)
        {
            var termsRevisionDate = DateTime.UtcNow.AddDays(-1);

            user.AcceptedTermsOfUseDate = DateTime.UtcNow;

            user.HasAcceptedLatestTermsOfUse(termsRevisionDate).Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasAcceptedLatestTermsOfUse_NewRevisionDate_False(AspNetUser user)
        {
            var termsRevisionDate = DateTime.UtcNow;

            user.AcceptedTermsOfUseDate = termsRevisionDate.AddDays(-5);

            user.HasAcceptedLatestTermsOfUse(termsRevisionDate).Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasAcceptedLatestTermsOfUse_NotAccepted_False(AspNetUser user)
        {
            var termsRevisionDate = DateTime.UtcNow;

            user.AcceptedTermsOfUseDate = null;

            user.HasAcceptedLatestTermsOfUse(termsRevisionDate).Should().BeFalse();
        }
    }
}
