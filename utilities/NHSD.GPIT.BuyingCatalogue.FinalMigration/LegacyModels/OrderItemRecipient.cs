using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class OrderItemRecipient
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public string OdsCode { get; set; }
        public int Quantity { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
