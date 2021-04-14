using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class PublicationStatus
    {
        public PublicationStatus()
        {
            CatalogueItems = new HashSet<CatalogueItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CatalogueItem> CatalogueItems { get; set; }
    }
}
