using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    [Table("AspNetUserClaims")]
    public partial class AspNetUserClaim : IdentityUserClaim<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
