using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class DefaultDeliveryDate
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public DateTime DeliveryDate { get; set; }        
    }
}
