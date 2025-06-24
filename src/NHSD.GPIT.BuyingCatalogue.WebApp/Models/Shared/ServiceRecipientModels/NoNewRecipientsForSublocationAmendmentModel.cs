using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class NoNewRecipientsForSublocationAmendmentModel : NavBaseModel
    {
        public NoNewRecipientsForSublocationAmendmentModel()
        {
        }

        public NoNewRecipientsForSublocationAmendmentModel(
            Order order,
            IReadOnlyList<string> previousOrderRecipients,
            string backLinkUrl)
        {
            Title = "Add service recipients";
            Caption = order.CallOffId.ToString();
            Advice = "No new service recipients can be selected for this sublocation";

            BackLink = backLinkUrl;

            PreviousOrderRecipients = previousOrderRecipients;
            PreviousNounPhrase = order.Revision == 2 ? "the previous revision" : "previous revisions";

            SaveAndContinueLink = backLinkUrl;
        }

        public IReadOnlyList<string> PreviousOrderRecipients { get; init; }

        public string PreviousNounPhrase { get; init; }

        public string SaveAndContinueLink { get; init; }
    }
}
