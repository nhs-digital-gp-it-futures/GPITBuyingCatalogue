using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
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
        }

        public int PrimaryOrganisationId { get; set; }

        public Organisation PrimaryOrganisation { get; set; }

        public bool Disabled { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public bool HasOptedInUserResearch { get; set; }

        public DateTime? AcceptedTermsOfUseDate { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

        public ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

        public ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

        public ICollection<AspNetUserToken> AspNetUserTokens { get; set; }

        public bool HasAcceptedLatestTermsOfUse(DateTime revisionDate)
            => AcceptedTermsOfUseDate.GetValueOrDefault() >= revisionDate;
    }
}
