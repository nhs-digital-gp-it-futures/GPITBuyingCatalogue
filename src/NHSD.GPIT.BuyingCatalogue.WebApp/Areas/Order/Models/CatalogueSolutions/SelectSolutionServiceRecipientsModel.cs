using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionServiceRecipientsModel : OrderingBaseModel
    {
        public SelectSolutionServiceRecipientsModel()
        {
        }

        public SelectSolutionServiceRecipientsModel(string odsCode, string callOffId, string solutionName, IList<OrderItemRecipientModel> serviceRecipients, string selectionMode, bool isNewOrder, string catalogueSolutionId)
        {
            if (isNewOrder)
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution";
            else
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/{catalogueSolutionId}";

            BackLinkText = "Go back";
            Title = $"Service Recipients for {solutionName} for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            ServiceRecipients = serviceRecipients.ToList();

            if (selectionMode != null)
            {
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
        }

        public string SelectionPrompt { get; set; } = "Select all";

        public string SelectionParameter { get; set; } = "all";

        public string CallOffId { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }
    }
}
