using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public record ServiceRecipientModel
    {
        public ServiceRecipientModel()
        {
        }

        public ServiceRecipientModel(ServiceRecipient serviceRecipientServiceModel)
        {
            Name = serviceRecipientServiceModel.Name;
            OdsCode = serviceRecipientServiceModel.OrgId;
            Location = serviceRecipientServiceModel.Location;
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public string Description => $"{Name}" + (string.IsNullOrEmpty(OdsCode) ? string.Empty : $" ({OdsCode})");

        public string Location { get; set; }
    }
}
