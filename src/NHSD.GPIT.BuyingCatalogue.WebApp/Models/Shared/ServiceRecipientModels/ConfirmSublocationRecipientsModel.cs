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
            string backLinkUrl,
            string continueLinkUrl)
        {
            Title = "Confirm service recipients";
            Caption = competition.Name;
            Advice = "Review the organisations you've selected to receive the winning solution for this competition.";

            Sublocations = competition.CompetitionSublocations
                .Select(x => new SublocationModel(x, false))
                .ToArray();

            BackLink = backLinkUrl;

            AddRemoveRecipientsLink = BackLink;
            SaveAndContinueLink = continueLinkUrl;
        }

        public string AddRemoveRecipientsLink { get; init; }

        public string SaveAndContinueLink { get; init; }

        public IReadOnlyCollection<SublocationModel> Sublocations { get; init; }
    }
}
