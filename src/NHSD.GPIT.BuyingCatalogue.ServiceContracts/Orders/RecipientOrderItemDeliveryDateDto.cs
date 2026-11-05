using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

public class RecipientOrderItemDeliveryDateDto
{
    public required int OrderSublocationRecipientId { get; init; }

    public CallOffId CallOffId { get; init; }

    public DateTime? DeliveryDate { get; set; }
}
