﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class FrameworkCapability : IAudited
    {
        public string FrameworkId { get; set; }

        public int CapabilityId { get; set; }

        public bool IsFoundation { get; set; }

        public Capability Capability { get; set; }

        public Framework Framework { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}
