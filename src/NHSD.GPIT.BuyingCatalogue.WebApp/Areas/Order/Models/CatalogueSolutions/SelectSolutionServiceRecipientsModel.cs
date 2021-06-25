﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class SelectSolutionServiceRecipientsModel : OrderingBaseModel
    {
        public SelectSolutionServiceRecipientsModel()
        {
        }

        public SelectSolutionServiceRecipientsModel(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            IEnumerable<OrderItemRecipientModel> serviceRecipients,
            string selectionMode,
            bool isNewOrder,
            CatalogueItemId catalogueSolutionId)
        {
            BackLink = isNewOrder
                ? $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution"
                : $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/{catalogueSolutionId}";

            BackLinkText = "Go back";
            Title = $"Service Recipients for {solutionName} for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            ServiceRecipients = serviceRecipients.ToList();

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
