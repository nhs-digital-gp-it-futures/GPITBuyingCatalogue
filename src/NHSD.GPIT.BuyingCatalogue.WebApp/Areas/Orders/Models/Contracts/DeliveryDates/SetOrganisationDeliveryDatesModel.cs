using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class SetOrganisationDeliveryDatesModel : NavBaseModel
{
    public SetOrganisationDeliveryDatesModel()
    {
    }

    [SetsRequiredMembers]
    public SetOrganisationDeliveryDatesModel(
        string internalOrgId,
        CallOffId callOffId,
        int orderItemId,
        string recipientOrganisationId,
        IEnumerable<OrderItemSublocationRecipient> recipientOrderItems,
        Order order)
    {
        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
        OrderItemId = orderItemId;
        RecipientOrganisationId = recipientOrganisationId;
        RecipientOrganisationName = recipientOrderItems.First()?.Recipient?.RecipientOdsOrganisation?.Name ?? string.Empty;
        OrderItemsDeliveryDates = [.. recipientOrderItems.Select(x => new OrderItemDeliveryDate(x.Id, x.OrderItem.CatalogueItem.Name, x.DeliveryDate, order))];
    }

    public required string InternalOrgId { get; set; }

    public required CallOffId CallOffId { get; set; }

    public required int OrderItemId { get; set; }

    public string RecipientOrganisationId { get; set; }

    public string RecipientOrganisationName { get; set; }

    public List<string> SelectedRecipients { get; set; }

    public List<OrderItemDeliveryDate> OrderItemsDeliveryDates { get; set; }

    public string GetPageTitle()
    {
        return $"Change delivery dates for {RecipientOrganisationName}";
    }
}
