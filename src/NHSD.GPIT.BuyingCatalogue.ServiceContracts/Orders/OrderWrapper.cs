using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderWrapper
    {
        private readonly List<Order> previous;

        public OrderWrapper(IEnumerable<Order> orders)
        {
            var ordered = orders.OrderBy(x => x.CallOffId.Revision).ToList();

            Order = ordered.Any()
                ? ordered.Last()
                : null;

            previous = ordered.Count > 1
                ? ordered.SkipLast(1).ToList()
                : new List<Order>();
        }

        public Order Order { get; set; }

        public Order Last => previous.Any() ? previous.Last() : null;

        public bool HasPreviousVersions => previous.Any();

        public bool AssociatedServicesOnly => Order.AssociatedServicesOnly;

        public OrderItem Solution => AllOrders.Select(x => x.GetSolution()).FirstOrDefault(x => x != null);

        public CatalogueItemId? SolutionId => AllOrders.Select(x => x.GetSolutionId()).FirstOrDefault(x => x != null);

        private IEnumerable<Order> AllOrders => previous.Union(new[] { Order })
            .OrderByDescending(x => x.Revision)
            .ToList();

        public IEnumerable<OrderItem> AdditionalServices()
        {
            var output = new List<OrderItem>();

            foreach (var service in AllOrders.SelectMany(x => x.GetAdditionalServices()))
            {
                if (output.Any(x => x.CatalogueItemId == service.CatalogueItemId))
                {
                    continue;
                }

                output.Add(service);
            }

            return output;
        }

        public IEnumerable<OrderItem> AssociatedServices()
        {
            var output = new List<OrderItem>();

            foreach (var service in AllOrders.SelectMany(x => x.GetAssociatedServices()))
            {
                if (output.Any(x => x.CatalogueItemId == service.CatalogueItemId))
                {
                    continue;
                }

                output.Add(service);
            }

            return output;
        }

        public bool HasPreviousServiceRecipientFor(CatalogueItemId catalogueItemId, string odsCode)
        {
            return previous.Any(x => x.OrderItems.Any(oi => oi.CatalogueItemId == catalogueItemId && oi.OrderItemRecipients.Any(r => r.OdsCode == odsCode)));
        }

        public OrderItem OrderItem(CatalogueItemId catalogueItemId)
        {
            return AllOrders.Select(x => x.OrderItem(catalogueItemId)).FirstOrDefault(x => x != null);
        }
    }
}
