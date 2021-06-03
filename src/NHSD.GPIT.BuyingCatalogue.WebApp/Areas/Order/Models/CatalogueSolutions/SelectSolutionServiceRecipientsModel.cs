﻿using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionServiceRecipientsModel : OrderingBaseModel
    {
        public SelectSolutionServiceRecipientsModel()
        {
        }

        public SelectSolutionServiceRecipientsModel(string odsCode, string callOffId, string solutionName, IList<ServiceRecipientsModel> serviceRecipients, string selectionMode)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution";
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
                        c.Checked = true;
                        return true;
                    });
                    SelectionPrompt = "Deselect all";
                    SelectionParameter = "none";
                }
                else if (selectionMode.Equals("none", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    ServiceRecipients.All(c =>
                    {
                        c.Checked = false;
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

        public IList<ServiceRecipientsModel> ServiceRecipients { get; set; }
    }
}
