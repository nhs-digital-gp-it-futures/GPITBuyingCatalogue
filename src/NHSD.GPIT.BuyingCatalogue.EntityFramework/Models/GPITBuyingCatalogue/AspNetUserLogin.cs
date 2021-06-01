using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [Table("AspNetUserLogins")]
    public partial class AspNetUserLogin : IdentityUserLogin<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
