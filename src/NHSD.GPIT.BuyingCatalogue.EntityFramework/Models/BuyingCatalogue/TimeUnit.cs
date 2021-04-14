using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class TimeUnit
    {
        public TimeUnit()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        public int TimeUnitId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CataloguePrice> CataloguePrices { get; set; }
    }
}
