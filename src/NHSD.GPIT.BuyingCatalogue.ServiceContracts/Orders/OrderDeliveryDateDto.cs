using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderDeliveryDateDto
    {
        public OrderDeliveryDateDto()
        {
        }

        public OrderDeliveryDateDto(string odsCode, DateTime deliveryDate)
        {
            OdsCode = odsCode;
            DeliveryDate = deliveryDate;
        }

        public string OdsCode { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}
