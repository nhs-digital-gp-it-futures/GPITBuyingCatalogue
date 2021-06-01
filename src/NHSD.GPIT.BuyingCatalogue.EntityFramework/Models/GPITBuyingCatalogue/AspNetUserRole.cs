using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [Table("AspNetUserRoles")]
    public partial class AspNetUserRole : IdentityUserRole<string>
    {
        public virtual AspNetRole Role { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
