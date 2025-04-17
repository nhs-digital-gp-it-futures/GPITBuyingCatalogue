using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public record OrderSublocationRecipient : ISublocationRecipient
    {
        public int OrderId { get; set; }

        public string RecipientOdsCode { get; set; }

        public string ParentSublocationOdsCode { get; set; }

        public Order Order { get; set; }

        public OrderSublocation ParentSublocation { get; set; }

        public OdsOrganisation RecipientOdsOrganisation { get; set; }

        public ICollection<OrderItemSublocationRecipient> OrderItemSublocationRecipients { get; set; } =
            new HashSet<OrderItemSublocationRecipient>();
    }
}
