using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class NoNewRecipientsForAmendmentModel : NavBaseModel
    {
        public NoNewRecipientsForAmendmentModel()
        {
        }

        public NoNewRecipientsForAmendmentModel(
            Order order,
            string backLinkUrl,
            string continueLinkUrl)
        {
            Title = "Confirm service recipients";
            Caption = order.Description;
            Advice = "No new service recipients have been selected for this amendment";

            BackLink = backLinkUrl;

            AddRemoveRecipientsLink = BackLink;
            SaveAndContinueLink = continueLinkUrl;
        }

        public string AddRemoveRecipientsLink { get; init; }

        public string SaveAndContinueLink { get; init; }
    }
}
