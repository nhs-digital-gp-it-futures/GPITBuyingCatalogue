using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

        public EditDatesModel(OrderWrapper orderWrapper, CatalogueItemId catalogueItemId, RoutingSource? source = null)
        {
            var order = orderWrapper.Order;
            InternalOrgId = order.OrderingParty.InternalIdentifier;
            CallOffId = order.CallOffId;
            CatalogueItemId = catalogueItemId;
            DeliveryDate = order.DeliveryDate;
            Source = source;
            DisplayEditLink = order.GetPreviousOrderItemId(catalogueItemId) == null;

            var orderItem = order.OrderItem(catalogueItemId);

            CatalogueItemType = orderItem.CatalogueItem.CatalogueItemType;
            Description = orderItem.CatalogueItem.Name;

            Recipients = orderWrapper.DetermineOrderRecipients(catalogueItemId)
                .Select(x => new RecipientDateModel(x, x.GetDeliveryDateForItem(orderItem.CatalogueItemId) ?? DeliveryDate, order.CommencementDate!.Value))
                .ToArray();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public RoutingSource? Source { get; set; }

        public string Description { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool DisplayEditLink { get; set; }

        public RecipientDateModel[] Recipients { get; set; }
    }
}
