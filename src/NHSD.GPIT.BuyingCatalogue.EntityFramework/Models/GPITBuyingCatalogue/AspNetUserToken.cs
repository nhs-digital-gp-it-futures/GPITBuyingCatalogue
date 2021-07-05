using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [ExcludeFromCodeCoverage]
    [Table("AspNetUserTokens")]
    public partial class AspNetUserToken : IdentityUserToken<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
