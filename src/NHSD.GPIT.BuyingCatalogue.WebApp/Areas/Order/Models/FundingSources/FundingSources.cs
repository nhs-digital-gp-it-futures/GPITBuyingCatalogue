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

            var catSol = order.OrderItems.FirstOrDefault(oi => oi.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            /* if there is a solution - check if any of its frameworks make it editable
            if they do, add everything to the editable list, if not, add cat sol and addit serv to uneditable list and only add assoc services to editable list
            else if there is no cat sol, would be an assoc service order and just dump everything into the editable list */
            if (catSol is not null)
            {
                if (!catSol.ItemIsLocalFundingOnly())
                {
                    OrderItemsSelectable.AddRange(order.OrderItems);
                }
                else
                {
                    OrderItemsLocalOnly.AddRange(order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService));
                    OrderItemsSelectable.AddRange(order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService));
                }
            }
            else
            {
                OrderItemsSelectable.AddRange(order.OrderItems);
            }
        }

        public CallOffId CallOffId { get; set; }

        public string Caption { get; set; }

        public List<OrderItem> OrderItemsSelectable { get; set; }

        public List<OrderItem> OrderItemsLocalOnly { get; set; }
    }
}
