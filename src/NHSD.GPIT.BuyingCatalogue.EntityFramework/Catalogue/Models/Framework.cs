using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Framework
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public string Owner { get; set; }

        public DateTime? ActiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
