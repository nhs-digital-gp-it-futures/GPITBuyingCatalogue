using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

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
            : this(backLinkUrl, continueLinkUrl)
        {
            Caption = competition.Name;
            Advice = "Review the organisations you've selected to receive the winning solution for this competition.";

            Sublocations = competition.CompetitionSublocations
                .Select(cs => new SublocationModel(cs, false))
                .ToArray();
        }

        public ConfirmSublocationRecipientsModel(
            Order order,
            string backLinkUrl,
            string continueLinkUrl)
            : this(backLinkUrl, continueLinkUrl)
        {
            Caption = order.Description;
            Advice = "Review the organisations you've selected to receive the winning solution for this order.";

            Sublocations = order.OrderSublocations
                .Select(os => new SublocationModel(os, false))
                .ToArray();
        }

        private ConfirmSublocationRecipientsModel(
            string backLinkUrl,
            string continueLinkUrl)
        {
            Title = "Confirm service recipients";

            BackLink = backLinkUrl;

            AddRemoveRecipientsLink = BackLink;
            SaveAndContinueLink = continueLinkUrl;
        }

        public string AddRemoveRecipientsLink { get; init; }

        public string SaveAndContinueLink { get; init; }

        public IReadOnlyCollection<SublocationModel> Sublocations { get; init; }
    }
}
