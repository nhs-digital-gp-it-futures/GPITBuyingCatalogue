using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [ExcludeFromCodeCoverage]
    [Table("AspNetUserClaims")]
    public partial class AspNetUserClaim : IdentityUserClaim<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
