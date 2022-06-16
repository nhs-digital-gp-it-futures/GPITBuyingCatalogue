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
            Title = "Select funding sources";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            Caption = $"Order {CallOffId}";

            OrderItemsSelectable = new List<OrderItem>();
            OrderItemsLocalOnly = new List<OrderItem>();
            OrderItemsNoFundingRequired = new List<OrderItem>(0);

            var catSol = order.OrderItems.FirstOrDefault(oi => oi.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderItemsNoFundingRequired = order.OrderItems.Where(oi => oi.CurrentFundingType() == OrderItemFundingType.NoFundingRequired).ToList();
            OrderItemsLocalOnly = order.OrderItems.Where(oi => oi.CurrentFundingType() == OrderItemFundingType.LocalFundingOnly).ToList();
            OrderItemsSelectable = order.OrderItems.Where(oi => !oi.IsCurrentlyForcedFunding()).ToList();
        }

        public CallOffId CallOffId { get; set; }

        public string Caption { get; set; }

        public List<OrderItem> OrderItemsSelectable { get; set; }

        public List<OrderItem> OrderItemsLocalOnly { get; set; }

        public List<OrderItem> OrderItemsNoFundingRequired { get; set; }
    }
}
