using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [Table("AspNetUserTokens")]
    public partial class AspNetUserToken : IdentityUserToken<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
