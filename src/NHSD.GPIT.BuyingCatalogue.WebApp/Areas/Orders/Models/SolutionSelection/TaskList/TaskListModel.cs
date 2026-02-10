using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
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
        private readonly Dictionary<CatalogueItemId, TaskListOrderItemModel> taskModelsForPrevious = new();

        public TaskListModel()
        {
        }

        public TaskListModel(
            string internalOrgId,
            CallOffId callOffId,
            OrderWrapper wrapper)
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
            CatalogueSolution = rolledUpOrder.GetSolutionOrderItem();
            AdditionalServices = rolledUpOrder.GetAdditionalServices();
            AssociatedServices = wrapper.Order.GetAssociatedServices() ?? new List<OrderItem>();
            PreviousAssociatedServices = Previous?.GetAssociatedServices() ?? new List<OrderItem>();
            HasNewRecipients = wrapper.HasNewOrderRecipients;

            var currentAdditionalServices = wrapper.Order.GetAdditionalServices();

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
                        rolledUpOrder.FlattenedRecipients,
                        CatalogueSolution)
                    {
                        FromPreviousRevision = Previous?.Exists(CatalogueSolution.CatalogueItemId) ?? false,
                        HasNewRecipients = wrapper.HasNewOrderRecipients,
                        NumberOfPrices = CatalogueSolution.CatalogueItem.CataloguePrices.Count,
                        PriceId = CatalogueSolution.CatalogueItem.CataloguePrices.Count == 1
                            ? CatalogueSolution.CatalogueItem.CataloguePrices.First().CataloguePriceId
                            : 0,
                        PreviousRecipients = Previous?.FlattenedRecipients.Count() ?? 0,
                        QuantityChanged =
                            (Previous?.OrderItems ?? [])
                            .FirstOrDefault(x => x.CatalogueItemId == CatalogueSolution.CatalogueItemId)
                            ?.Quantity != wrapper.Order.GetSolutionOrderItem().Quantity,
                        CanBeRemoved = false,
                    });
            }

            AdditionalServices.ForEach(x => taskModels.Add(
                x.CatalogueItemId,
                new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, rolledUpOrder.FlattenedRecipients, x)
                {
                    FromPreviousRevision = Previous?.Exists(x.CatalogueItemId) ?? false,
                    HasNewRecipients = wrapper.HasNewOrderRecipients,
                    NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                    PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                        ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                        : 0,
                    PreviousRecipients = Previous?.FlattenedRecipients.Count() ?? 0,
                    QuantityChanged = ((Previous?.OrderItems ?? [])
                            .FirstOrDefault(y => y.CatalogueItemId == x.CatalogueItemId)
                            ?.Quantity ?? 0) !=
                        (currentAdditionalServices?.Where(y => y.CatalogueItemId == x.CatalogueItemId)
                            .FirstOrDefault()
                            ?.Quantity ?? 0),
                    CanBeRemoved = !(IsAmendment && (Previous?.Exists(x.CatalogueItemId) ?? false)),
                }));

            AssociatedServices.ForEach(x => AddTaskModelForAssociatedService(taskModels, internalOrgId, callOffId, x, rolledUpOrder, !OrderType.MergerOrSplit));
            PreviousAssociatedServices.ForEach(x => AddTaskModelForAssociatedService(taskModelsForPrevious, internalOrgId, callOffId, x, rolledUpOrder, false));
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

        public IEnumerable<OrderItem> PreviousAssociatedServices { get; set; }

        public TaskProgress Progress =>
            taskModels.All(x =>
                x.Value.PriceStatus is TaskProgress.Completed
                && x.Value.QuantityStatus is TaskProgress.Completed or TaskProgress.Amended)
                ? TaskProgress.Completed
                : TaskProgress.InProgress;

        public string OnwardLink { get; set; }

        public TaskListOrderItemModel OrderItemModel(CatalogueItemId catalogueItemId) => taskModels.TryGetValue(catalogueItemId, out TaskListOrderItemModel value) ? value : null;

        public TaskListOrderItemModel OrderItemModelForPrevious(CatalogueItemId catalogueItemId) => taskModelsForPrevious.TryGetValue(catalogueItemId, out TaskListOrderItemModel value) ? value : null;

        private void AddTaskModelForAssociatedService(
            Dictionary<CatalogueItemId, TaskListOrderItemModel> models,
            string internalOrgId,
            CallOffId callOffId,
            OrderItem x,
            Order rolledUpOrder,
            bool canBeRemoved)
        {
            models.Add(
                x.CatalogueItemId,
                new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, rolledUpOrder.FlattenedRecipients, x)
                {
                    FromPreviousRevision = x.Order.CallOffId.Revision < callOffId.Revision,
                    HasNewRecipients = HasNewRecipients,
                    NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                    PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                        ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                        : 0,
                    CanBeRemoved = canBeRemoved,
                });
        }
    }
}
