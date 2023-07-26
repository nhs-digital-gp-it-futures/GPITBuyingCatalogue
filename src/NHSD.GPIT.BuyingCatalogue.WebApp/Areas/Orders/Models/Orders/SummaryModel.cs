using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public sealed class SummaryModel : OrderingBaseModel
    {
        public SummaryModel(OrderWrapper orderWrapper, string internalOrgId, ImplementationPlan defaultPlan)
        {
            InternalOrgId = internalOrgId;
            OrderWrapper = orderWrapper;
            DefaultPlan = defaultPlan;
        }

        public OrderWrapper OrderWrapper { get; }

        public CallOffId CallOffId => OrderWrapper.Order.CallOffId;

        public Order Order => OrderWrapper.Order;

        public Order Previous => OrderWrapper.Previous;

        public Order RolledUp => OrderWrapper.RolledUp;

        public string AdviceText { get; set; }

        public bool CanBeAmended { get; set; }

        public ImplementationPlan DefaultPlan { get; set; }

        public ImplementationPlan BespokePlan => Order.Contract?.ImplementationPlan;

        public ContractBilling BespokeBilling => Order.Contract?.ContractBilling;

        public bool HasBespokeMilestones => BespokePlan != null && BespokePlan.Milestones.Any();

        public string DefaultMilestoneLabelText => "Default milestones and payment triggers";

        public string BespokeMilestoneLabelText => "Bespoke milestones and payment triggers";

        public bool HasBespokeBilling => BespokeBilling != null && BespokeBilling.ContractBillingItems.Any();

        public bool HasSpecificRequirements => BespokeBilling != null && BespokeBilling.Requirements.Any();

        public string BespokeBillingLabelText => "Bespoke milestones and payment triggers";

        public string RequirementLabelText => "Specific requirements";

        public FundingTypeDescriptionModel FundingTypeDescription(CatalogueItemId catalogueItemId)
        {
            return new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(catalogueItemId));
        }
    }
}
