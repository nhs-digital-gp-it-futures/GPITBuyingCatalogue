using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList
{
    public class TaskListModel : NavBaseModel
    {
        public const string AmendmentTitle = "Amend items from the previous order";
        public const string CompletedTitle = "Edit solutions and services";
        public const string MergerSplitTitle = "Edit associated service";
        public const string InProgressTitle = "Catalogue solution and services";

        private readonly Dictionary<CatalogueItemId, TaskListOrderItemModel> taskModels = new();
        private readonly Dictionary<CallOffId, Dictionary<CatalogueItemId, TaskListOrderItemModel>> taskModelsForPrevious = new();

        public TaskListModel()
        {
        }

        public TaskListModel(
            string internalOrgId,
            CallOffId callOffId,
            OrderWrapper wrapper,
            IDictionary<CatalogueItemId, int> associatedServicesForAdditionalServices)
        {
            var rolledUpOrder = wrapper?.RolledUp;

            if (rolledUpOrder == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            Previous = wrapper.PreviousOrders.LastOrDefault();

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            OrderType = rolledUpOrder.OrderType;
            CatalogueSolution = wrapper.Order.GetSolutionOrderItem();
            AdditionalServices = wrapper.Order.GetAdditionalServices();
            AssociatedServices = wrapper.Order.GetAssociatedServices() ?? new List<OrderItem>();
            PreviousAssociatedServices = wrapper.PreviousOrders
                .SelectMany(order => order.GetAssociatedServices() ?? new List<OrderItem>())
                .GroupBy(item => item.Order.CallOffId);
            HasNewRecipients = wrapper.HasNewOrderRecipients;

            if (rolledUpOrder.OrderType.AssociatedServicesOnly)
            {
                SolutionName = rolledUpOrder.AssociatedServicesOnlyDetails.Solution?.Name;
            }

            if (CatalogueSolution != null)
            {
                taskModels.Add(
                    CatalogueSolution.CatalogueItemId,
                    new TaskListOrderItemModel(
                        internalOrgId,
                        callOffId,
                        OrderType,
                        wrapper.DetermineOrderRecipients(CatalogueSolution.CatalogueItemId),
                        CatalogueSolution)
                    {
                        FromPreviousRevision = Previous?.Exists(CatalogueSolution.CatalogueItemId) ?? false,
                        HasNewRecipients = wrapper.HasNewOrderRecipients,
                        NumberOfPrices = CatalogueSolution.CatalogueItem.CataloguePrices.Count,
                        PriceId = CatalogueSolution.CatalogueItem.CataloguePrices.Count == 1
                            ? CatalogueSolution.CatalogueItem.CataloguePrices.First().CataloguePriceId
                            : 0,
                        PreviousRecipients = Previous?.FlattenedRecipients.Count() ?? 0,
                        CanBeRemoved = false,
                        Source = RoutingSource.TaskList,
                        OrderItemId = CatalogueSolution.Id,
                    });
            }

            AdditionalServices.ForEach(x => taskModels.Add(
                x.CatalogueItemId,
                new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, wrapper.DetermineOrderRecipients(x.CatalogueItemId), x)
                {
                    FromPreviousRevision = Previous?.Exists(x.CatalogueItemId) ?? false,
                    HasNewRecipients = wrapper.HasNewOrderRecipients,
                    NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                    AssociatedServicesCatalogueItemsCount = associatedServicesForAdditionalServices.TryGetValue(x.CatalogueItemId, out var associatedServicesCount) ? associatedServicesCount : 0,
                    PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                        ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                        : 0,
                    PreviousRecipients = Previous?.FlattenedRecipients.Count() ?? 0,
                    AssociatedServicesOrderItems = x.Services.ToList(),
                    CanBeRemoved = !(IsAmendment && (Previous?.Exists(x.CatalogueItemId) ?? false)),
                    Source = RoutingSource.TaskList,
                    OrderItemId = x.Id,
                }));

            AssociatedServices.ForEach(x => AddTaskModelForAssociatedService(
                taskModels,
                internalOrgId,
                callOffId,
                x,
                wrapper.DetermineOrderRecipients(x.CatalogueItemId)
                    .ToList(),
                !OrderType.MergerOrSplit));

            PreviousAssociatedServices.ForEach(grouping =>
            {
                var groupingDict = grouping.ToDictionary(
                    item => item.CatalogueItemId,
                    item => BuildTaskListOrderItemModelForAssociatedService(
                        internalOrgId,
                        callOffId,
                        item,
                        wrapper.DetermineOrderRecipients(item.CatalogueItemId).ToList(),
                        false));

                taskModelsForPrevious.Add(
                    grouping.Key,
                    groupingDict);
            });
        }

        public Order Previous { get; set; }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool HasNewRecipients { get; set; }

        public OrderType OrderType { get; set; }

        public override string Title => IsAmendment
            ? AmendmentTitle
            : OrderType.MergerOrSplit
                ? MergerSplitTitle
                : Progress == TaskProgress.Completed
                    ? CompletedTitle
                    : InProgressTitle;

        public override string Caption => OrderType.MergerOrSplit
            ? SolutionName
            : $"Order {CallOffId}";

        public string SolutionName { get; set; }

        public OrderItem CatalogueSolution { get; set; }

        public bool AlternativeSolutionsAvailable { get; set; }

        public bool AdditionalServicesAvailable { get; set; }
        
        public bool UnselectedAdditionalServicesAvailable { get; set; }

        public IEnumerable<OrderItem> AdditionalServices { get; set; }

        public bool AssociatedServicesAvailable { get; set; }

        public bool UnselectedAssociatedServicesAvailable { get; set; }

        public IEnumerable<OrderItem> AssociatedServices { get; set; }

        public IEnumerable<IGrouping<CallOffId, OrderItem>> PreviousAssociatedServices { get; set; }

        public TaskProgress Progress =>
            taskModels.All(x =>
                x.Value.PriceStatus is TaskProgress.Completed
                && x.Value.QuantityStatus is TaskProgress.Completed or TaskProgress.Amended)
                ? TaskProgress.Completed
                : TaskProgress.InProgress;

        public string OnwardLink { get; set; }

        public TaskListOrderItemModel OrderItemModel(CatalogueItemId catalogueItemId) => taskModels.TryGetValue(catalogueItemId, out TaskListOrderItemModel value) ? value : null;

        public TaskListOrderItemModel OrderItemModelForPrevious(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            if (taskModelsForPrevious.TryGetValue(callOffId, out var catalogueItemDict))
            {
                var model = catalogueItemDict.TryGetValue(catalogueItemId, out var taskListOrderItemModel) ? taskListOrderItemModel : null;
                model?.HasNewRecipients = false;

                return model;
            }

            return null;
        }

        private void AddTaskModelForAssociatedService(
            Dictionary<CatalogueItemId, TaskListOrderItemModel> models,
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            List<OrderSublocationRecipient> recipients,
            bool canBeRemoved)
        {
            models.Add(
                orderItem.CatalogueItemId,
                BuildTaskListOrderItemModelForAssociatedService(internalOrgId, callOffId, orderItem, recipients, canBeRemoved));
        }

        private TaskListOrderItemModel BuildTaskListOrderItemModelForAssociatedService(string internalOrgId, CallOffId callOffId, OrderItem orderItem, List<OrderSublocationRecipient> recipients, bool canBeRemoved)
        {
            return new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, recipients, orderItem)
            {
                FromPreviousRevision = orderItem.Order.CallOffId.Revision < callOffId.Revision,
                HasNewRecipients = HasNewRecipients,
                NumberOfPrices = orderItem.CatalogueItem.CataloguePrices.Count,
                PriceId = orderItem.CatalogueItem.CataloguePrices.Count == 1
                    ? orderItem.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
                CanBeRemoved = canBeRemoved,
                Source = RoutingSource.TaskList,
                OrderItemId = orderItem.Id,
            };
        }
    }
}
