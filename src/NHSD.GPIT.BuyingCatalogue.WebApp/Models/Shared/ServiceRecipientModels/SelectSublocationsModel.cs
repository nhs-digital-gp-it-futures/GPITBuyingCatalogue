using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationsModel : NavBaseModel
    {
        public SelectSublocationsModel(Competition competition)
        {
            Title = "Select sublocations for this order";
            Caption = competition.Name;
            Advice = $"Select all the {competition.Organisation.Name} sublocations that will be receiving this order";
        }
    }
}
