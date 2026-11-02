using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;

public interface IDeliveryDateService
{
    public Task SetAllDeliveryDates(string internalOrgId, CallOffId callOffId, DateTime deliveryDate);

    public Task ResetDeliveryDates(int orderId, DateTime commencementDate);

    public Task<IEnumerable<OrderSublocationRecipient>> GetSelectedRecipientsAsync(CallOffId callOffId, IEnumerable<string> selectedRecipients);

    public Task<IEnumerable<OrderItemSublocationRecipient>> GetRecipientOrderItemsDeliveryDatesAsync(CallOffId callOffId, string recipientId);

    public Task SetRecipientOrderItemDeliveryDatesAsync(IEnumerable<RecipientOrderItemDeliveryDateDto> deliveryDates);

    public Task<DateTime?> GetMostCommonRecipientDeliveryDateAsync(CallOffId callOffId);
}
