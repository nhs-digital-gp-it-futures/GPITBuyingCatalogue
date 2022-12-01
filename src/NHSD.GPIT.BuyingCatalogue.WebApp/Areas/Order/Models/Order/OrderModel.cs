using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class OrderModel : OrderingBaseModel
    {
        public OrderModel(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress orderSections,
            string organisationName = "")
        {
            InternalOrgId = internalOrgId;
            Progress = orderSections;

            if (order is null)
            {
                Title = "New order";
                TitleAdvice = "You must provide an order description before a unique ID is created for this order.";
                OrganisationName = organisationName;
            }
            else
            {
                Title = $"Order {order.CallOffId}";
                CallOffId = order.CallOffId;
                IsAmendment = order.IsAmendment;
                TitleAdvice = order.IsAmendment
                    ? "You can amend parts of this order as required and will need to review other parts that cannot be changed. Your amendments will be saved as you progress through each section."
                    : "Complete the following steps to create an order summary.";
                Description = order.Description;
                OrganisationName = order.OrderingParty.Name;

                LastUpdatedByUserName = order.LastUpdatedByUser.FullName;
                LastUpdated = order.LastUpdated;
                ShowSelectFrameworkPage = string.IsNullOrWhiteSpace(order.SelectedFrameworkId);
            }
        }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public string Description { get; set; }

        public string DescriptionUrl { get; set; }

        public string TitleAdvice { get; set; }

        public string OrganisationName { get; set; }

        public string LastUpdatedByUserName { get; set; }

        public DateTime? LastUpdated { get; set; }

        public OrderProgress Progress { get; set; }

        public bool ShowSelectFrameworkPage { get; set; }
    }
}
