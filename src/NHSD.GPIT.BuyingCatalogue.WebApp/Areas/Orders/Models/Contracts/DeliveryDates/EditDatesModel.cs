using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class EditDatesModel : NavBaseModel
    {
        public EditDatesModel()
        {
        }

        public EditDatesModel(OrderWrapper orderWrapper, CatalogueItemId catalogueItemId, IReadOnlyDictionary<string, string> organisations, RoutingSource? source = null)
        {
            var order = orderWrapper.Order;
            InternalOrgId = order.OrderingParty.InternalIdentifier;
            CallOffId = order.CallOffId;
            OrderType = order.OrderType;
            SolutionName = order.OrderType.GetSolutionNameFromOrder(order);
            PracticeReorganisationName = order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient?.Name ?? string.Empty;
            CatalogueItemId = catalogueItemId;
            DeliveryDate = order.DeliveryDate;
            Source = source;
            DisplayEditLink = order.GetPreviousOrderItemId(catalogueItemId) == null;

            var orderItem = order.OrderItem(catalogueItemId);

            CatalogueItemType = orderItem.CatalogueItem.CatalogueItemType;
            Description = orderItem.CatalogueItem.Name;

            var recipients = orderWrapper.DetermineOrderRecipients(catalogueItemId)
                .Select(x => new RecipientDateModel(x, x.GetDeliveryDateForItem(orderItem.CatalogueItemId) ?? DeliveryDate, order.CommencementDate!.Value, organisations[x.OdsCode]))
                .OrderBy(y => y.Description)
                .ToArray();

            Recipients = OrderType.MergerOrSplit ?
                new List<KeyValuePair<string, RecipientDateModel[]>> { new(OrderType.Value == OrderTypeEnum.AssociatedServiceSplit ? "Service Recipients receiving patients" : "Service Recipients to be merged", recipients) } :
                recipients
                    .GroupBy(x => x.Location)
                    .Select(
                        x => new KeyValuePair<string, RecipientDateModel[]>(
                            x.Key,
                            x.OrderBy(y => y.Description).ToArray()))
                    .ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderType OrderType { get; set; }

        public string SolutionName { get; set; }

        public string PracticeReorganisationName { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public RoutingSource? Source { get; set; }

        public string Description { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool DisplayEditLink { get; set; }

        public List<KeyValuePair<string, RecipientDateModel[]>> Recipients { get; set; }
    }
}
