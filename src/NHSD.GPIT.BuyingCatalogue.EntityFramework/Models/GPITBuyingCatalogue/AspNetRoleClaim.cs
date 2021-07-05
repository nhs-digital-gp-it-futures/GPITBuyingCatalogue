using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [ExcludeFromCodeCoverage]
    [Table("AspNetRoleClaims")]
    public partial class AspNetRoleClaim : IdentityRoleClaim<string>
    {
        public virtual AspNetRole Role { get; set; }
    }
}
