using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class RemoveSublocationsModel : NavBaseModel
    {
        public RemoveSublocationsModel()
        {
        }

        public RemoveSublocationsModel(
            Competition competition,
            IReadOnlyList<string> sublocations,
            IReadOnlyList<string> removes,
            string backLink)
            : this(sublocations, removes, backLink)
        {
            Caption = competition.Name;
            Advice = "Confirm you want to remove sublocations from this competition";
            ListHeaderText = $"{competition.Organisation.Name} {Pluralisation} to be removed:";
        }

        public RemoveSublocationsModel(
            Order order,
            IReadOnlyList<string> sublocations,
            IReadOnlyList<string> removes,
            string backLink)
            : this(sublocations, removes, backLink)
        {
            Caption = order.Description;
            Advice = "Confirm you want to remove sublocations from this order";
            ListHeaderText = $"{order.OrderingParty.Name} {Pluralisation} to be removed:";
            BackLink = backLink;
        }

        private RemoveSublocationsModel(
            IReadOnlyList<string> sublocations,
            IReadOnlyList<string> removes,
            string backLink)
        {
            SublocationOdsCodes = sublocations;
            Removes = removes;
            Pluralisation = Removes.Count == 1
                ? "sublocation"
                : "sublocations";

            Title = $"Remove {Pluralisation}";
            BackLink = backLink;
        }

        public bool? ConfirmRemove { get; init; }

        public IEnumerable<SelectOption<bool>> ConfirmRemoveOptions => new List<SelectOption<bool>>
        {
            new($"Yes, I want to remove the {Pluralisation}", true),
            new($"No, I want to keep the {Pluralisation}", false),
        };

        public string ListHeaderText { get; init; }

        public string Pluralisation { get; init; }

        public IReadOnlyList<string> SublocationOdsCodes { get; init; }

        public IReadOnlyList<string> Removes { get; init; }
    }
}
