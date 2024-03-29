﻿using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models
{
    [Serializable]
    public sealed class Address
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        public string Line4 { get; set; }

        public string Line5 { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }
    }
}
