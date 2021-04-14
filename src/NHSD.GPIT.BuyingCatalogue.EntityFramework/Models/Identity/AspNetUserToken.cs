using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity
{    
    [Table("AspNetUserTokens")]
    public partial class AspNetUserToken : IdentityUserToken<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
