using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipients
{
    public sealed class SelectSolutionServiceRecipientsModel : OrderingBaseModel
    {
        public SelectSolutionServiceRecipientsModel()
        {
        }

        public SelectSolutionServiceRecipientsModel(
            string odsCode,
            CreateOrderItemModel state,
            string selectionMode)
        {
            BackLink = state.IsNewSolution
                ? $"/order/organisation/{odsCode}/order/{state.CallOffId}/catalogue-solutions/select/solution"
                : $"/order/organisation/{odsCode}/order/{state.CallOffId}/catalogue-solutions/{state.CatalogueItemId}";

            BackLinkText = "Go back";
            Title = $"Service Recipients for {state.CatalogueItemName} for {state.CallOffId}";
            OdsCode = odsCode;
            CallOffId = state.CallOffId;
            ServiceRecipients = state.ServiceRecipients;

            if (selectionMode is null)
                return;

            if (selectionMode.Equals("all", System.StringComparison.InvariantCultureIgnoreCase))
            {
                ServiceRecipients.All(c =>
                {
                    c.Selected = true;
                    return true;
                });
                SelectionPrompt = "Deselect all";
                SelectionParameter = "none";
            }
            else if (selectionMode.Equals("none", System.StringComparison.InvariantCultureIgnoreCase))
            {
                ServiceRecipients.All(c =>
                {
                    c.Selected = false;
                    return true;
                });
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
