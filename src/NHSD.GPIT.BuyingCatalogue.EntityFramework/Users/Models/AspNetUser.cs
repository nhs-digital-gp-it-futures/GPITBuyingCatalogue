using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [Table("AspNetUsers")]
    public partial class AspNetUser : IdentityUser<string>
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
        }

        public Guid PrimaryOrganisationId { get; set; }

        public string OrganisationFunction { get; set; }

        public bool Disabled { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
    }
}
