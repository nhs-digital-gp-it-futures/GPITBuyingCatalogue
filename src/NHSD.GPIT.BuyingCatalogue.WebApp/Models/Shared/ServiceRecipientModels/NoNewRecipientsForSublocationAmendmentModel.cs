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
            Caption = order.Description;
            Advice = "No new service recipients can be selected for this sublocation";

            BackLink = backLinkUrl;

            PreviousOrderRecipients = previousOrderRecipients;

            SaveAndContinueLink = backLinkUrl;
        }

        public IReadOnlyList<string> PreviousOrderRecipients { get; init; }
        public string SaveAndContinueLink { get; init; }
    }
}
