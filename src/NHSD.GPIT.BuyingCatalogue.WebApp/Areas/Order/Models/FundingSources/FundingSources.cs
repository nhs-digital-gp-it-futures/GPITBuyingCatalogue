using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources
{
    public sealed class FundingSources : OrderingBaseModel
    {
        public FundingSources()
        {
        }

        public FundingSources(string internalOrgId, CallOffId callOffId, EntityFramework.Ordering.Models.Order order)
        {
            Title = "Funding sources";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            Caption = $"Order {CallOffId}";
            MaximumTerm = order.MaximumTerm!.Value;

            var completedOrderItems = order.OrderItems.Where(oi => oi.AllQuantitiesEntered).ToList();

            OrderItemsNoFundingRequired = completedOrderItems.Where(oi => oi.FundingType == OrderItemFundingType.NoFundingRequired).ToList();
            OrderItemsLocalOnly = completedOrderItems.Where(oi => oi.FundingType == OrderItemFundingType.LocalFundingOnly).ToList();
            OrderItemsSelectable = completedOrderItems.Where(oi => !oi.IsForcedFunding).ToList();
        }

        public CallOffId CallOffId { get; set; }

        public string Caption { get; set; }

        public List<OrderItem> OrderItemsSelectable { get; set; }

        public List<OrderItem> OrderItemsLocalOnly { get; set; }

        public List<OrderItem> OrderItemsNoFundingRequired { get; set; }

        public int MaximumTerm { get; set; }
    }
}
