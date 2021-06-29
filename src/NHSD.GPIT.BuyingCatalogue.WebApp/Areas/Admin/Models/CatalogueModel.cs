using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class CatalogueModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime LastUpdated { get; set; }

        public string PublishedStatus { get; set; }

        public int PublishedStatusId { get; set; }

        public string Supplier { get; set; }
    }
}
