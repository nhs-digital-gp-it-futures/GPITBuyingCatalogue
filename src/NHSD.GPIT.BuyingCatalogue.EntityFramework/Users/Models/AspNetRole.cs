﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models
{
    [Serializable]
    public sealed class AspNetRole : IdentityRole<int>
    {
        public AspNetRole()
        {
            AspNetRoleClaims = new HashSet<AspNetRoleClaim>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
        }

        public ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
