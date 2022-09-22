using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates
{
    public class RecipientDateModel : DateInputModel
    {
        public RecipientDateModel()
        {
        }

        public RecipientDateModel(OrderItemRecipient recipient)
        {
            OdsCode = recipient.OdsCode;
            Description = recipient.Recipient.Name;
            SetDateFields(recipient.DeliveryDate);
        }

        public string OdsCode { get; set; }

        public string Description { get; set; }
    }
}
