using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

public class OrderRecipient
{
    public int OrderId { get; set; }

    public string OdsCode { get; set; }

    public Order Order { get; set; }

    public OdsOrganisation OdsOrganisation { get; set; }

    public ICollection<OrderItemRecipient> OrderItemRecipients { get; set; }
}
