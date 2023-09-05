﻿using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectQuantityProvider : IRoutingResultProvider
    {
        private readonly IAssociatedServicesService associatedServicesService;

        public SelectQuantityProvider(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
        }

        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

            if (routeValues?.CatalogueItemId == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            var orderItem = order.OrderItem(routeValues.CatalogueItemId.Value);
            var attentionRequired = order.IsAmendment
                && !orderWrapper.DetermineOrderRecipients(orderItem.CatalogueItemId)
                                .AllDeliveryDatesEntered(orderItem.CatalogueItemId);

            if (routeValues.Source == RoutingSource.TaskList
                && !attentionRequired)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.TaskList,
                    ActionName = Constants.Actions.TaskList,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            if (routeValues.CallOffId.IsAmendment)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.DeliveryDates,
                    ActionName = Constants.Actions.AmendDeliveryDate,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        routeValues.CatalogueItemId,
                        routeValues.Source,
                    },
                };
            }

            if (order.GetAssociatedServices().Any())
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.Review,
                    ActionName = Constants.Actions.Review,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var associatedServices = associatedServicesService.GetPublishedAssociatedServicesForSolution(order.GetSolutionId()).Result;

            if (associatedServices.Any())
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.AssociatedServices,
                    ActionName = Constants.Actions.AddAssociatedServices,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            return new RoutingResult
            {
                ControllerName = Constants.Controllers.Review,
                ActionName = Constants.Actions.Review,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }
    }
}
