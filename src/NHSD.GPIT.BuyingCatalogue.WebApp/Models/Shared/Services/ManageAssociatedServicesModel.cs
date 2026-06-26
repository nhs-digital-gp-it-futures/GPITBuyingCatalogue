using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

public class ManageAssociatedServicesModel(
    List<OrderItem> associatedServices,
    string catalogueItemName,
    IEnumerable<OrderSublocationRecipient> recipients,
    CallOffId callOffId,
    string internalOrgId,
    CatalogueItemId additionalServiceId,
    OrderWrapper orderWrapper) : NavBaseModel
{
    public IEnumerable<OrderItem> AssociatedServices { get; set; } = associatedServices;

    public string CatalogueItemName { get; set; } = catalogueItemName;

    public IEnumerable<OrderSublocationRecipient> Recipients { get; set; } = recipients;

    public CallOffId CallOffId { get; set; } = callOffId;

    public string InternalOrgId { get; set; } = internalOrgId;

    public CatalogueItemId AdditionalServiceId { get; set; } = additionalServiceId;

    public bool UnselectedAssociatedServicesAvailable { get; set; }

    public bool IsAmendment { get; set; } = orderWrapper.IsAmendment;

    public bool HasNewRecipients { get; set; } = orderWrapper.HasNewOrderRecipients;

    public IDictionary<CallOffId, List<OrderItem>> PreviousAssociatedServices => orderWrapper
        .PreviousOrders
        .SelectMany(order => order.GetAdditionalServices() ?? new List<OrderItem>())
        .Where(additionalService => additionalService.CatalogueItemId == AdditionalServiceId)
        .GroupBy(additionalService => additionalService.Order.CallOffId, additionalService => additionalService)
        .ToDictionary(
            grouping => grouping.Key,
            grouping => grouping.FirstOrDefault()?.Services.ToList() ?? []);

    public TaskListOrderItemModel BuildAssociatedServiceOrderItemModel(OrderItem associatedServiceOrderItem)
    {
        return new TaskListOrderItemModel(InternalOrgId, CallOffId, OrderTypeEnum.Solution, Recipients, associatedServiceOrderItem)
        {
            FromPreviousRevision = false,
            CanBeRemoved = true,
            OrderItemId = associatedServiceOrderItem.Id,
            Source = RoutingSource.ManageAssociatedServices,
            NumberOfPrices = associatedServiceOrderItem.CatalogueItem.CataloguePrices.Count,
            HasNewRecipients = HasNewRecipients,
            PriceId = associatedServiceOrderItem.CatalogueItem.CataloguePrices.Count == 1
                ? associatedServiceOrderItem.CatalogueItem.CataloguePrices.First().CataloguePriceId
                : 0,
        };
    }
}
