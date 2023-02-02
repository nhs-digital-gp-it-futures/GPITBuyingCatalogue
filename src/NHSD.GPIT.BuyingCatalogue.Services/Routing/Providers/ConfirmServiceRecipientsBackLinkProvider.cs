using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class ConfirmServiceRecipientsBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(Order order, RouteValues routeValues)
        {
            if (routeValues?.Journey == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            var actionName = routeValues.Journey == JourneyType.Add
                ? Constants.Actions.AddServiceRecipients
                : Constants.Actions.EditServiceRecipients;

            return new RoutingResult
            {
                ActionName = actionName,
                ControllerName = Constants.Controllers.ServiceRecipients,
                RouteValues = new
                {
                    routeValues.InternalOrgId,
                    routeValues.CallOffId,
                    routeValues.CatalogueItemId,
                    routeValues.Source,
                    routeValues.RecipientIds,
                },
            };
        }
    }
}
