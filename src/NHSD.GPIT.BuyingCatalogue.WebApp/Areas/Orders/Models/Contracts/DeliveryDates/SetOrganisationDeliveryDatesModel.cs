using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class SetOrganisationDeliveryDatesModel : NavBaseModel
{
    public SetOrganisationDeliveryDatesModel()
    {
    }

    public SetOrganisationDeliveryDatesModel(
        string internalOrgId,
        CallOffId callOffId,
        int orderItemId,
        string recipientOrganisationId,
        IEnumerable<OrderItemSublocationRecipient> recipientOrderItems)
    {
        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
        OrderItemId = orderItemId;
        RecipientOrganisationId = recipientOrganisationId;
        RecipientOrganisationName = recipientOrderItems.First().Recipient.RecipientOdsOrganisation?.Name;
    }

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }

    public int OrderItemId { get; set; }

    public string RecipientOrganisationId { get; set; }

    public string RecipientOrganisationName { get; set; }

    public List<string> SelectedRecipients { get; set; }

    public string GetPageTitle()
    {
        return $"Change delivery dates for {RecipientOrganisationName}";
    }
}
