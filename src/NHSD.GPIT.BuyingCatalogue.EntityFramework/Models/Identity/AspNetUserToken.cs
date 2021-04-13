using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity
{
    [ExcludeFromCodeCoverage]
    [Table("AspNetUserTokens")]
    public partial class AspNetUserToken : IdentityUserToken<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
