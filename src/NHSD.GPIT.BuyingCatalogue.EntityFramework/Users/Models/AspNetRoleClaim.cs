﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class AspNetRoleClaim : IdentityRoleClaim<Guid>
    {
        public AspNetRole Role { get; set; }
    }
}
