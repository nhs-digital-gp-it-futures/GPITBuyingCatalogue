using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public record OrderSublocation : ISublocation
    {
        public OrderSublocation()
        {
        }

        public OrderSublocation(int orderId, string sublocationOdsCode, string ownerOdsCode)
        {
            OrderId = orderId;
            SublocationOdsCode = sublocationOdsCode;
            OwnerOdsCode = ownerOdsCode;
        }

        public int OrderId { get; set; }

        public string SublocationOdsCode { get; set; }

        public string OwnerOdsCode { get; set; }

        public Order Order { get; set; }

        public ICollection<OrderSublocationRecipient> SublocationRecipients { get; set; } = [];

        public OdsOrganisation SublocationOrganisation { get; set; }
    }
}
