﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class AspNetUserLogin : IdentityUserLogin<Guid>
    {
        public AspNetUser User { get; set; }
    }
}
