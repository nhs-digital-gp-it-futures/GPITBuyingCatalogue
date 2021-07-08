using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class SelectAdditionalServiceRecipientsModel : OrderingBaseModel
    {
        public SelectAdditionalServiceRecipientsModel()
        {
        }

        public SelectAdditionalServiceRecipientsModel(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            IEnumerable<OrderItemRecipientModel> serviceRecipients,
            string selectionMode,
            bool isNewOrder,
            CatalogueItemId additionalServiceId)
        {
            if (isNewOrder)
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service";
            else
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/{additionalServiceId}";

            BackLinkText = "Go back";
            Title = $"Service Recipients for {solutionName} for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            ServiceRecipients = serviceRecipients.ToList();

            if (selectionMode is null)
                return;

            if (selectionMode.Equals("all", System.StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var recipient in ServiceRecipients)
                    recipient.Selected = true;

                SelectionPrompt = "Deselect all";
                SelectionParameter = "none";
            }
            else if (selectionMode.Equals("none", System.StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var recipient in ServiceRecipients)
                    recipient.Selected = false;

                SelectionPrompt = "Deselect all";
                SelectionParameter = "none";
            }
        }

        public string SelectionPrompt { get; set; } = "Select all";

        public string SelectionParameter { get; set; } = "all";

        public CallOffId CallOffId { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }
    }
}
