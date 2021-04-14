using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class AssociatedService
    {
        public string AssociatedServiceId { get; set; }
        public string Description { get; set; }
        public string OrderGuidance { get; set; }
        public DateTime? LastUpdated { get; set; }
        public Guid? LastUpdatedBy { get; set; }

        public virtual CatalogueItem AssociatedServiceNavigation { get; set; }
    }
}
