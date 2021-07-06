using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [Table("AspNetRoles")]
    public class AspNetRole : IdentityRole<string>
    {
        public AspNetRole()
        {
            AspNetRoleClaims = new HashSet<AspNetRoleClaim>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
        }

        public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
