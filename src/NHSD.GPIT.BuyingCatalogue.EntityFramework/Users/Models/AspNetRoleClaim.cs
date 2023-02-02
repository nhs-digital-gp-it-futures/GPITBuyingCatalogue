using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    [Serializable]
    public sealed class AspNetRoleClaim : IdentityRoleClaim<int>
    {
        public AspNetRole Role { get; set; }
    }
}
