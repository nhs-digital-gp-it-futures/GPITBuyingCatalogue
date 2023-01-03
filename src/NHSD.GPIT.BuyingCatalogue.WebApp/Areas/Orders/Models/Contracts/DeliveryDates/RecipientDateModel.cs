using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class RecipientDateModel : DateInputModel
    {
        public RecipientDateModel()
        {
        }

        public RecipientDateModel(OrderItemRecipient recipient, DateTime commencementDate)
        {
            OdsCode = recipient.OdsCode;
            Description = recipient.Recipient.Name;
            CommencementDate = commencementDate;

            SetDateFields(recipient.DeliveryDate);
        }

        public string OdsCode { get; set; }

        public DateTime CommencementDate { get; set; }
    }
}
