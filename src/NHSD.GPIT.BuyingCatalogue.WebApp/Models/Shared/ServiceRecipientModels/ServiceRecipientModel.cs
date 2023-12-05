using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public class ServiceRecipientModel
    {
        public string OdsCode { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public string Description => $"{Name}" + (string.IsNullOrEmpty(OdsCode) ? string.Empty : $" ({OdsCode})");

        public string Location { get; set; }

        public ServiceRecipientDto Dto => new()
        {
            Name = Name,
            OdsCode = OdsCode,
            Location = Location,
        };
    }
}
