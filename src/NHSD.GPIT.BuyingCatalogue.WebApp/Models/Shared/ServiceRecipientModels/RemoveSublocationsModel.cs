using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class RemoveSublocationsModel : NavBaseModel
    {
        public RemoveSublocationsModel(
            Competition competition,
            HashSet<string> sublocationsToRemove,
            HashSet<string> sublocationsToAdd)
        {
            SublocationIdsToRemove = sublocationsToRemove;
            SublocationIdsToAdd = sublocationsToAdd;
            Pluralisation = SublocationIdsToRemove is { Count: 1 }
                ? "sublocation"
                : "sublocations";

            Title = $"Remove {Pluralisation}";
            Caption = competition.Name;
            Advice = "Confirm you want to remove sublocations from this order";
            ListHeaderText = $"{competition.Organisation.Name} {Pluralisation} to be removed:";
        }

        public bool ConfirmRemove { get; init; }

        public string ListHeaderText { get; init; }

        public string Pluralisation { get; init; }

        public HashSet<string> SublocationIdsToRemove { get; init; }

        public HashSet<string> SublocationIdsToAdd { get; init; }
    }
}
