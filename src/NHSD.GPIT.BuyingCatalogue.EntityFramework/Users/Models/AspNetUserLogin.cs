using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    [Table("AspNetUserLogins")]
    public partial class AspNetUserLogin : IdentityUserLogin<string>
    {
        public virtual AspNetUser User { get; set; }
    }
}
