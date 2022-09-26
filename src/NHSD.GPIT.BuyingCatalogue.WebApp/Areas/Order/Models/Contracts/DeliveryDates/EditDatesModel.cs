﻿using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates
{
    public class EditDatesModel : NavBaseModel
    {
        public EditDatesModel()
        {
        }

        public EditDatesModel(EntityFramework.Ordering.Models.Order order, CatalogueItemId catalogueItemId, RoutingSource? source = null)
        {
            InternalOrgId = order.OrderingParty.InternalIdentifier;
            CallOffId = order.CallOffId;
            CatalogueItemId = catalogueItemId;
            DeliveryDate = order.DeliveryDate;
            Source = source;

            var orderItem = order.OrderItem(catalogueItemId);

            CatalogueItemType = orderItem.CatalogueItem.CatalogueItemType;
            Description = orderItem.CatalogueItem.Name;

            Recipients = orderItem.OrderItemRecipients
                .Select(x => new RecipientDateModel(x, order.CommencementDate!.Value))
                .ToArray();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public RoutingSource? Source { get; set; }

        public string Description { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public RecipientDateModel[] Recipients { get; set; }
    }
}
