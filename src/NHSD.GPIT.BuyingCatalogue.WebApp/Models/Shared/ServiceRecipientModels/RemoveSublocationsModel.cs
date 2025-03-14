using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class RemoveSublocationsModel : NavBaseModel
    {
        public RemoveSublocationsModel(Competition competition)
        {
            Title = "Remove sublocations";
            Caption = competition.Name;
            Advice = "Confirm you want to remove sublocations from this order";
            ListHeaderText = $"{competition.Organisation.Name} sublocations to be removed:";
        }

        public bool ConfirmRemove { get; set; }

        public string ListHeaderText { get; set; }

        public HashSet<string> SublocationIdsToRemove { get; set; }

        public HashSet<string> SublocationIdsToAdd { get; set; }
    }
}
