using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            LocationOrgId = serviceRecipientServiceModel.LocationOrgId;
        }

        public ServiceRecipientModel(
            CompetitionSublocationRecipient competitionSublocationRecipientEntityModel,
            bool selected)
        {
            Name = competitionSublocationRecipientEntityModel.RecipientOrganisation?.Name;
            OdsCode = competitionSublocationRecipientEntityModel.RecipientOdsCode;
            Location = competitionSublocationRecipientEntityModel.ParentSublocation?.SublocationOrganisation?.Name;
            LocationOrgId = competitionSublocationRecipientEntityModel.ParentSublocationOdsCode;
            Selected = selected;
        }

        public ServiceRecipientModel(
            OrderSublocationRecipient orderSublocationRecipientEntityModel,
            bool selected)
        {
            Name = orderSublocationRecipientEntityModel.RecipientOdsOrganisation?.Name;
            OdsCode = orderSublocationRecipientEntityModel.RecipientOdsCode;
            Location = orderSublocationRecipientEntityModel.ParentSublocation?.SublocationOrganisation?.Name;
            LocationOrgId = orderSublocationRecipientEntityModel.ParentSublocationOdsCode;
            Selected = selected;
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public string Description => $"{Name}" + (string.IsNullOrEmpty(OdsCode) ? string.Empty : $" ({OdsCode})");

        public string Location { get; set; }

        public string LocationOrgId { get; set; }
    }
}
