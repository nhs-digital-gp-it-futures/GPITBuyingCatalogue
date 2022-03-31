using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ServiceRecipients
{
    public class ServiceRecipientModel
    {
        public string OdsCode { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public ServiceRecipientDto Dto => new()
        {
            Name = Name,
            OdsCode = OdsCode,
        };
    }
}
