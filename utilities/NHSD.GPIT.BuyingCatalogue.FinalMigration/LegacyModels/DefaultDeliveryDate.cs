using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class DefaultDeliveryDate
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public DateTime DeliveryDate { get; set; }        
    }
}
