using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients
{
    public sealed class SelectAdditionalServiceRecipientsModel : OrderingBaseModel
    {
        public SelectAdditionalServiceRecipientsModel()
        {
        }

        public SelectAdditionalServiceRecipientsModel(
            string internalOrgId,
            CreateOrderItemModel state,
            string selectionMode)
        {
            BackLink = state.IsNewSolution
                ? state.SkipPriceSelection
                 ? $"/order/organisation/{internalOrgId}/order/{state.CallOffId}/additional-services/select/additional-service"
                 : $"/order/organisation/{internalOrgId}/order/{state.CallOffId}/additional-services/select/additional-service/price"
                : $"/order/organisation/{internalOrgId}/order/{state.CallOffId}/additional-services/{state.CatalogueItemId}";

            Title = $"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}";
            InternalOrgId = internalOrgId;
            CallOffId = state.CallOffId;
            ServiceRecipients = state.ServiceRecipients;

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

                SelectionPrompt = "Select all";
                SelectionParameter = "all";
            }
        }

        public string SelectionPrompt { get; set; } = "Select all";

        public string SelectionParameter { get; set; } = "all";

        public CallOffId CallOffId { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }
    }
}
