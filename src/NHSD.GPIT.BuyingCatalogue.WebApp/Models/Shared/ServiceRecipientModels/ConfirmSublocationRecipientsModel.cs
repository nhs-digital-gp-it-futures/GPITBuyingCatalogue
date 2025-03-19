using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class ConfirmSublocationRecipientsModel : NavBaseModel
    {
        public ConfirmSublocationRecipientsModel()
        {
        }

        public ConfirmSublocationRecipientsModel(
            Competition competition,
            string addRemoveRecipientsLink)
        {
            Title = "Confirm service recipients)";
            Caption = competition.Name;
            Advice = "Review the organisations you've selected to receive the winning solution for this competition.";

            Sublocations = competition.CompetitionSublocations.SelectMany(
                x => new List<SublocationModel> { new(x, false) });

            AddRemoveRecipientsLink = addRemoveRecipientsLink;
        }

        public string AddRemoveRecipientsLink { get; init; }

        public IEnumerable<SublocationModel> Sublocations { get; init; } = [];
    }
}
