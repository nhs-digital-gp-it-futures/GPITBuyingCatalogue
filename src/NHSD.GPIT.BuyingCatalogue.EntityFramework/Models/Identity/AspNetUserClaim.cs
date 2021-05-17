using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity
{
    [Table("AspNetUserClaims")]
    public partial class AspNetUserClaim : IdentityUserClaim<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
