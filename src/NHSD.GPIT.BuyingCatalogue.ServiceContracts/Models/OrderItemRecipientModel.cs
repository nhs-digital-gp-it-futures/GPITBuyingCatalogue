using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class OrderItemRecipientModel
    {
        public OrderItemRecipientModel()
        {
        }

        public OrderItemRecipientModel(ServiceRecipient recipient)
        {
            Name = recipient.Name;
            OdsCode = recipient.OrgId;
        }

        // MJRTODO - Checked is probably a bad name now its going all the way to the service
        public bool Checked { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string Name { get; init; }

        public string OdsCode { get; init; }

        public int? Quantity { get; set; }

        public decimal? CostPerYear { get; init; }
    }
}
