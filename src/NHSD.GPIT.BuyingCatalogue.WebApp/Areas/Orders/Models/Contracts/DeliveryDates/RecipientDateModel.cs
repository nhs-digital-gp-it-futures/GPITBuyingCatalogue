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

        public RecipientDateModel(
            OrderSublocationRecipient recipient,
            DateTime? deliveryDate,
            DateTime commencementDate)
        {
            OdsCode = recipient.RecipientOdsCode;
            Description = recipient.RecipientOdsOrganisation?.Name;
            CommencementDate = commencementDate;

            SetDateFields(deliveryDate);
            Location = recipient.ParentSublocation.SublocationOrganisation.Name;
        }

        public string OdsCode { get; set; }

        public DateTime CommencementDate { get; set; }

        public string Location { get; set; }
    }
}
