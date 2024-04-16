using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [Serializable]
    public sealed class AspNetUser : IdentityUser<int>, IAudited
    {
        public static class ExpiryThresholds
        {
            public static List<(int Threshold, EventTypeEnum Event)> ThresholdsMap = new()
            {
                (30, EventTypeEnum.PasswordEnteredFirstExpiryThreshold),
                (14, EventTypeEnum.PasswordEnteredSecondExpiryThreshold),
                (1, EventTypeEnum.PasswordEnteredThirdExpiryThreshold),
            };
        }

        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
        }

        public int PrimaryOrganisationId { get; set; }

        public Organisation PrimaryOrganisation { get; set; }

        public bool Disabled { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public DateTime PasswordUpdatedDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public bool HasOptedInUserResearch { get; set; }

        public DateTime? AcceptedTermsOfUseDate { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

        public ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

        public ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

        public ICollection<AspNetUserToken> AspNetUserTokens { get; set; }

        public ICollection<AspNetUserEvent> Events { get; set; }

        public bool HasAcceptedLatestTermsOfUse(DateTime revisionDate)
            => AcceptedTermsOfUseDate.GetValueOrDefault() >= revisionDate;

        public int RemainingPasswordExpiryDays(DateTime date)
        {
            var remainingDays = Math.Max(0, (PasswordUpdatedDate.AddYears(1) - date).Days);

            return remainingDays;
        }

        public EventTypeEnum DetermineEventToRaise(DateTime today)
        {
            var remaining = RemainingPasswordExpiryDays(today);

            var eventToRaise = ExpiryThresholds.ThresholdsMap.OrderBy(x => x.Threshold)
                .Where(x => remaining <= x.Threshold)
                .Select(x => x.Event)
                .FirstOrDefault();

            return Events.Any(x => x.EventTypeId == (int)eventToRaise) || remaining == 0
                ? EventTypeEnum.Nothing
                : eventToRaise;
        }
    }
}
