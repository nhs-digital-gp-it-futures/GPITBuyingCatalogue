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

        public RecipientDateModel(OrderRecipient recipient, DateTime? deliveryDate, DateTime commencementDate)
        {
            OdsCode = recipient.OdsCode;
            Description = recipient.OdsOrganisation.Name;
            CommencementDate = commencementDate;

            SetDateFields(deliveryDate);
        }

        public string OdsCode { get; set; }

        public DateTime CommencementDate { get; set; }
    }
}
