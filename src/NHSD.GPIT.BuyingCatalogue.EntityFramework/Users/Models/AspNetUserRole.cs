using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    [Table("AspNetUserRoles")]
    public partial class AspNetUserRole : IdentityUserRole<string>
    {
        public virtual AspNetRole Role { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
