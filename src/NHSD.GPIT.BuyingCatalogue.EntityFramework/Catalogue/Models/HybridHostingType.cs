﻿using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public class HybridHostingType
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        [StringLength(1000)]
        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public bool IsValid() => !string.IsNullOrWhiteSpace(Summary) && !string.IsNullOrWhiteSpace(HostingModel);
    }
}
