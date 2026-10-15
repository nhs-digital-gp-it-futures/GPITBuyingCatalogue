using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

public class RecipientOrderItemDeliveryDateDto
{
    public required int OrderSublocationRecipientId { get; init; }

    public DateTime? DeliveryDate { get; set; }
}
