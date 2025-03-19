using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class RemoveSublocationsModel : NavBaseModel
    {
        public RemoveSublocationsModel()
        {
        }

        public RemoveSublocationsModel(
            Competition competition,
            IReadOnlyList<string> sublocationsToRemove,
            IReadOnlyList<string> sublocationsToAdd,
            string backLinkHref)
        {
            SublocationIdsToRemove = sublocationsToRemove;
            SublocationIdsToAdd = sublocationsToAdd;
            Pluralisation = SublocationIdsToRemove.Count == 1
                ? "sublocation"
                : "sublocations";

            Title = $"Remove {Pluralisation}";
            Caption = competition.Name;
            Advice = "Confirm you want to remove sublocations from this order";
            ListHeaderText = $"{competition.Organisation.Name} {Pluralisation} to be removed:";
            BackLink = backLinkHref;
        }

        public bool ConfirmRemove { get; init; }

        public string ListHeaderText { get; init; }

        public string Pluralisation { get; init; }

        public IReadOnlyList<string> SublocationIdsToRemove { get; init; }

        public IReadOnlyList<string> SublocationIdsToAdd { get; init; }
    }
}
