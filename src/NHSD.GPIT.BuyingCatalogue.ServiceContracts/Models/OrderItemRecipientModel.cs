using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class OrderItemRecipientModel
    {
        public DateTime? DeliveryDate { get; init; }

        public string Name { get; init; }

        public string OdsCode { get; init; }

        public int? Quantity { get; init; }

        public decimal? CostPerYear { get; init; }
    }
}
