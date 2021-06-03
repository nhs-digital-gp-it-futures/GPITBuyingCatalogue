using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class ServiceRecipientsModel : ServiceRecipient
    {
        public ServiceRecipientsModel()
        {
        }

        public ServiceRecipientsModel(ServiceRecipient recipient)
        {
            Name = recipient.Name;
            OrgId = recipient.OrgId;
            Status = recipient.Status;
            PrimaryRoleId = recipient.PrimaryRoleId;
        }

        public bool Checked { get; set; }

        public int? Quantity { get; set; }

        public DateTime? PlannedDeliveryDate { get; set; }
    }
}
