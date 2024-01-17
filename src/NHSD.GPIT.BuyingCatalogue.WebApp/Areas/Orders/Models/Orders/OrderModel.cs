using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public sealed class OrderModel : OrderingBaseModel
    {
        internal static readonly KeyValuePair<OrderSummaryField, string>[] MergerSplitSpecificDescriptions =
        {
            new(OrderSummaryField.SolutionsAndServices, "Select the solution involved, confirm the price and review the quantity you’re ordering."),
        };

        internal static readonly KeyValuePair<OrderSummaryField, string>[] AmendmentSpecificDescriptions =
        {
            new(OrderSummaryField.OrderDescription, "Update the description of this order if needed."),
            new(OrderSummaryField.OrderingParty, "Change the primary contact for this order if needed."),
            new(OrderSummaryField.Supplier, "Select a different supplier contact if needed."),
            new(OrderSummaryField.CommencementDate, "Review the commencement date, maximum term and initial period for this contract."),
            new(OrderSummaryField.FundingSources, "Allocate funding sources for items in this order."),
            new(OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete this amended order."),
        };

        internal static readonly KeyValuePair<OrderSummaryField, string>[] DefaultDescriptions =
        {
            new(OrderSummaryField.OrderDescription, "Provide a description of your order."),
            new(OrderSummaryField.OrderingParty, "Provide information about the primary contact for your order."),
            new(OrderSummaryField.Supplier, "Find the supplier you want to order from and select a supplier contact."),
            new(OrderSummaryField.CommencementDate, "Provide the commencement date, the length of the contract and its initial period."),
            new(OrderSummaryField.ServiceRecipients, "Select the organisations you want to receive this solution."),
            new(OrderSummaryField.SolutionsAndServices, "Select a solution or services, prices and quantities."),
            new(OrderSummaryField.PlannedDeliveryDates, "Enter the planned delivery dates for the items you're ordering."),
            new(OrderSummaryField.FundingSources, "Review how you’ll be paying for your order."),
            new(OrderSummaryField.ImplementationPlan, "Review the default milestones that will act as payment triggers and create bespoke ones."),
            new(OrderSummaryField.AssociatedServicesBilling, "Review the default milestones, create bespoke ones and add specific requirements."),
            new(OrderSummaryField.DataProcessing, "Download the data processing information template for the supplier to complete."),
            new(OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete your order."),
        };

        internal static readonly KeyValuePair<OrderSummaryField, string>[] CompetitionOrderDescriptions =
        {
            new(OrderSummaryField.OrderDescription, "Edit the description for this order if needed."),
            new(OrderSummaryField.Supplier, "Provide information about the supplier contact for your order."),
            new(OrderSummaryField.CommencementDate, "Review the maximum term of your contract and provide a commencement date and initial period."),
            new(OrderSummaryField.ServiceRecipients, "Review the organisations you’re ordering for."),
            new(OrderSummaryField.SolutionsAndServices, "Review the items you’re ordering and their prices and quantities."),
        };

        public OrderModel(
            string internalOrgId,
            OrderType orderType,
            OrderProgress orderSections,
            string organisationName)
            : this(internalOrgId, orderSections)
        {
            Title = "New order";
            TitleAdvice = "You must provide an order description before a unique ID is created for this order.";
            OrganisationName = organisationName;
            Descriptions = BuildDescriptions(orderType, false, false);
        }

        public OrderModel(
            string internalOrgId,
            OrderProgress orderSections,
            Order order)
            : this(internalOrgId, orderSections)
        {
            Title = $"Order {order.CallOffId}";
            CallOffId = order.CallOffId;
            IsCompetitionOrder = order.CompetitionId is not null;
            IsAmendment = order.IsAmendment;
            TitleAdvice = order.IsAmendment
                ? "You can amend parts of this order as required and will need to review other parts that cannot be changed. Your amendments will be saved as you progress through each section."
                    : IsCompetitionOrder
                        ? "The information you included in your competition has already been added. Your progress will be saved as you complete each section."
                : "Complete the following steps to create an order summary.";
            Description = order.Description;
            OrganisationName = order.OrderingParty.Name;

            LastUpdatedByUserName = order.LastUpdatedByUser.FullName;
            LastUpdated = order.LastUpdated;
            ShowSelectFrameworkPage = string.IsNullOrWhiteSpace(order.SelectedFrameworkId);
            Descriptions = BuildDescriptions(order.OrderType, order.IsAmendment, IsCompetitionOrder);
        }

        private OrderModel(
            string internalOrgId,
            OrderProgress orderSections)
        {
            InternalOrgId = internalOrgId;
            Progress = orderSections;
        }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; }

        public bool IsCompetitionOrder { get; set; }

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

        private ReadOnlyDictionary<OrderSummaryField, string> Descriptions { get; }

        public string StatusDescription(OrderSummaryField field)
        {
            return Descriptions.ContainsKey(field)
                ? Descriptions[field]
                : string.Empty;
        }

        private static void Apply(
            Dictionary<OrderSummaryField, string> descriptions,
            IEnumerable<KeyValuePair<OrderSummaryField, string>> overrideDescriptions)
        {
            foreach (var item in overrideDescriptions)
            {
                descriptions[item.Key] = item.Value;
            }
        }

        private ReadOnlyDictionary<OrderSummaryField, string> BuildDescriptions(OrderType orderType, bool isAmendment, bool isCompetitionOrder)
        {
            var descriptions = DefaultDescriptions.ToDictionary();

            if (orderType.MergerOrSplit)
            {
                Apply(descriptions, MergerSplitSpecificDescriptions);
            }

            if (isAmendment)
            {
                Apply(descriptions, AmendmentSpecificDescriptions);
            }
            else if (isCompetitionOrder)
            {
                Apply(descriptions, CompetitionOrderDescriptions);
            }

            return descriptions.AsReadOnly();
        }
    }
}
