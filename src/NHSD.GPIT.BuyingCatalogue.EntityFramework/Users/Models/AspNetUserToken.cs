using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    [Serializable]
    public sealed class AspNetUserToken : IdentityUserToken<int>
    {
        public AspNetUser User { get; set; }
    }
}
