using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    //public class SelectAdditionalServiceRecipientsModel : OrderingBaseModel
    //{
    //    public SelectAdditionalServiceRecipientsModel()
    //    {
    //        BackLink = "/order/organisation/03F/order/C010001-01/additional-services"; // TODO
    //        BackLinkText = "Go back";
    //        Title = "Service Recipients for Document Management for C010001-01"; // TODO
    //    }
    //}

    public class SelectAdditionalServiceRecipientsModel : OrderingBaseModel
    {
        public SelectAdditionalServiceRecipientsModel()
        {
        }

        public SelectAdditionalServiceRecipientsModel(string odsCode, string callOffId, string solutionName, IList<OrderItemRecipientModel> serviceRecipients, string selectionMode, bool isNewOrder, string catalogueSolutionId)
        {
            if (isNewOrder)
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/solution";
            else
                BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/{catalogueSolutionId}";

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
