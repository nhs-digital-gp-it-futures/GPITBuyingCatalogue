using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class RecipientDeliveryDateDto
    {
        public RecipientDeliveryDateDto()
        {
        }

        public RecipientDeliveryDateDto(string odsCode, DateTime deliveryDate)
        {
            OdsCode = odsCode;
            DeliveryDate = deliveryDate;
        }

        public string OdsCode { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}
