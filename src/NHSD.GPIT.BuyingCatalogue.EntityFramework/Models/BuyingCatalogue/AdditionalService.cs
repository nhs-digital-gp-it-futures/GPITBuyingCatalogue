using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class AdditionalService
    {
        public string CatalogueItemId { get; set; }
        public string Summary { get; set; }
        public string FullDescription { get; set; }
        public DateTime? LastUpdated { get; set; }
        public Guid? LastUpdatedBy { get; set; }
        public string SolutionId { get; set; }

        public virtual CatalogueItem CatalogueItem { get; set; }
        public virtual Solution Solution { get; set; }
    }
}
