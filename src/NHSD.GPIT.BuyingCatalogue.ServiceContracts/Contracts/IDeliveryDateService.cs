using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;

public interface IDeliveryDateService
{
    public Task SetDeliveryDate(string internalOrgId, CallOffId callOffId, DateTime deliveryDate);

    public Task SetAllDeliveryDates(string internalOrgId, CallOffId callOffId, DateTime deliveryDate);

    Task ResetRecipientDeliveryDates(int orderId);

    public Task SetDeliveryDates(int orderId, OrderItem orderItem, List<RecipientDeliveryDateDto> deliveryDates);

    public Task ResetDeliveryDates(int orderId, DateTime commencementDate);

    public Task<IEnumerable<OrderSublocationRecipient>> GetSelectedRecipientsAsync(int orderNumber, IEnumerable<string> selectedRecipients);

    public Task<IEnumerable<OrderItemSublocationRecipient>> GetRecipientOrderItemsDeliveryDatesAsync(int orderNumber, string recipientId);

    public Task SetRecipientOrderItemDeliveryDatesAsync(IEnumerable<RecipientOrderItemDeliveryDateDto> deliveryDates);
}
