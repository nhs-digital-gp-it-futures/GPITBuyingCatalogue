using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class RemoveSublocationsModel : NavBaseModel
    {
        public RemoveSublocationsModel(Competition competition)
        {
            Title = "Remove sublocations";
            Caption = competition.Organisation.Name;
            Advice = "Confirm you want to remove sublocations from this order";
        }
    }
}
