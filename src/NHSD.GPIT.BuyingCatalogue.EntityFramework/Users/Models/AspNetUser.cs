﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [Serializable]
    public sealed class AspNetUser : IdentityUser<int>, IAudited
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            Events = new HashSet<AspNetUserEvent>();
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

        public PasswordExpiryEventTypeEnum DetermineEventToRaise(DateTime today)
        {
            var remaining = RemainingPasswordExpiryDays(today);
            if (remaining == 0) return PasswordExpiryEventTypeEnum.Nothing;

            var eventToRaise = ExpiryThresholds.ThresholdsMap.OrderBy(x => x.Threshold)
                .Where(x => remaining <= x.Threshold)
                .Select(x => x.Event)
                .FirstOrDefault();

            return Events.Any(x => x.EventTypeId == (int)eventToRaise)
                ? PasswordExpiryEventTypeEnum.Nothing
                : eventToRaise;
        }

        [ExcludeFromCodeCoverage]
        public static class ExpiryThresholds
        {
            public static List<(int Threshold, PasswordExpiryEventTypeEnum Event)> ThresholdsMap => new()
            {
                (30, PasswordExpiryEventTypeEnum.PasswordEnteredFirstExpiryThreshold),
                (14, PasswordExpiryEventTypeEnum.PasswordEnteredSecondExpiryThreshold),
                (1, PasswordExpiryEventTypeEnum.PasswordEnteredThirdExpiryThreshold),
            };
        }
    }
}
