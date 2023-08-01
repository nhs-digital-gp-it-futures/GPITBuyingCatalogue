using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public sealed class OrderModel : OrderingBaseModel
    {
        private readonly Dictionary<OrderSummaryField, string> amendmentDescriptions = new()
        {
            { OrderSummaryField.OrderDescription, "Update the description of this order if needed." },
            { OrderSummaryField.OrderingParty, "Change the primary contact for this order if needed." },
            { OrderSummaryField.Supplier, "Select a different supplier contact if needed." },
            { OrderSummaryField.CommencementDate, "Review the commencement date, maximum term and initial period for this contract." },
            { OrderSummaryField.ServiceRecipients, "Select the organisations you want to receive the items you’re ordering." },
            { OrderSummaryField.SolutionsAndServices, "Add Service Recipients to items already included in this order or add Additional Services." },
            { OrderSummaryField.PlannedDeliveryDates, "Enter the planned delivery dates for the items you're ordering." },
            { OrderSummaryField.FundingSources, "Allocate funding sources for items in this order." },
            { OrderSummaryField.ImplementationPlan, "Review the default milestones that will act as payment triggers and create bespoke ones." },
            { OrderSummaryField.DataProcessing, "Download the data processing information template for the supplier to complete." },
            { OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete this amended order." },
        };

        private readonly Dictionary<OrderSummaryField, string> orderDescriptions = new()
        {
            { OrderSummaryField.OrderDescription, "Provide a description of your order." },
            { OrderSummaryField.OrderingParty, "Provide information about the primary contact for your order." },
            { OrderSummaryField.Supplier, "Find the supplier you want to order from and select a supplier contact." },
            { OrderSummaryField.CommencementDate, "Provide the commencement date, the length of the contract and its initial period." },
            { OrderSummaryField.ServiceRecipients, "Select the organisations you want to receive the items you’re ordering." },
            { OrderSummaryField.SolutionsAndServices, "Add Service Recipients, prices and quantities for what you’re ordering." },
            { OrderSummaryField.PlannedDeliveryDates, "Enter the planned delivery dates for the items you're ordering." },
            { OrderSummaryField.FundingSources, "Review how you’ll be paying for your order." },
            { OrderSummaryField.ImplementationPlan, "Review the default milestones that will act as payment triggers and create bespoke ones." },
            { OrderSummaryField.AssociatedServicesBilling, "Review the default milestones, create bespoke ones and add specific requirements." },
            { OrderSummaryField.DataProcessing, "Download the data processing information template for the supplier to complete." },
            { OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete your order." },
        };

        public OrderModel(
            string internalOrgId,
            Order order,
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

        public string DeleteButtonTitle => IsAmendment ? "Delete amendment" : "Delete order";

        public string SolutionsAndServicesSectionLabel => IsAmendment ? "Solution and services" : "Add solutions and services";

        public string OrganisationName { get; set; }

        public string LastUpdatedByUserName { get; set; }

        public DateTime? LastUpdated { get; set; }

        public OrderProgress Progress { get; set; }

        public bool ShowSelectFrameworkPage { get; set; }

        public string StatusDescription(OrderSummaryField field, bool isAmendment)
        {
            var descriptions = isAmendment
                ? amendmentDescriptions
                : orderDescriptions;

            return descriptions.ContainsKey(field)
                ? descriptions[field]
                : string.Empty;
        }
    }
}
