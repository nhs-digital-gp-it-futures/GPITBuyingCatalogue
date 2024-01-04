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
        public const string MergerSplitTitle = "Edit Associated Service";
        public const string InProgressTitle = "Catalogue Solution and services";

        private const string Add = "Add";
        private const string Change = "Change";

        private readonly Dictionary<CatalogueItemId, TaskListOrderItemModel> taskModels = new();

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

            var previous = wrapper.Previous;

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            OrderType = rolledUpOrder.OrderType;
            CatalogueSolution = rolledUpOrder.GetSolutionOrderItem();
            AdditionalServices = rolledUpOrder.GetAdditionalServices();
            AssociatedServices = rolledUpOrder.GetAssociatedServices();

            if (rolledUpOrder.OrderType.AssociatedServicesOnly)
            {
                SolutionName = rolledUpOrder.AssociatedServicesOnlyDetails.Solution?.Name;
            }

            if (CatalogueSolution != null)
            {
                taskModels.Add(CatalogueSolution.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, rolledUpOrder.OrderRecipients, CatalogueSolution)
                {
                    FromPreviousRevision = previous?.Exists(CatalogueSolution.CatalogueItemId) ?? false,
                    HasNewRecipients = wrapper.HasNewOrderRecipients,
                    NumberOfPrices = CatalogueSolution.CatalogueItem.CataloguePrices.Count,
                    PriceId = CatalogueSolution.CatalogueItem.CataloguePrices.Count == 1
                        ? CatalogueSolution.CatalogueItem.CataloguePrices.First().CataloguePriceId
                        : 0,
                });
            }

            AdditionalServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, rolledUpOrder.OrderRecipients, x)
            {
                FromPreviousRevision = previous?.Exists(x.CatalogueItemId) ?? false,
                HasNewRecipients = wrapper.HasNewOrderRecipients,
                NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                    ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
            }));

            AssociatedServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, OrderType, rolledUpOrder.OrderRecipients, x)
            {
                FromPreviousRevision = previous?.Exists(x.CatalogueItemId) ?? false,
                HasNewRecipients = wrapper.HasNewOrderRecipients,
                NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                    ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
            }));
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

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

        public IEnumerable<OrderItem> AdditionalServices { get; set; }

        public string AdditionalServicesActionText
        {
            get
            {
                var verb = IsAmendment
                    ? Add
                    : AdditionalServices?.Any() ?? false ? Change : Add;

                return $"{verb} Additional Services";
            }
        }

        public bool AssociatedServicesAvailable { get; set; }

        public IEnumerable<OrderItem> AssociatedServices { get; set; }

        public string AssociatedServicesActionText => $"{(AssociatedServices?.Any() ?? false ? Change : Add)} Associated Services";

        public TaskProgress Progress
        {
            get
            {
                if (taskModels.All(x => x.Value.PriceStatus == TaskProgress.Completed)
                    && taskModels.All(x => x.Value.QuantityStatus is TaskProgress.Completed or TaskProgress.Amended))
                {
                    return TaskProgress.Completed;
                }

                return TaskProgress.InProgress;
            }
        }

        public string OnwardLink { get; set; }

        public TaskListOrderItemModel OrderItemModel(CatalogueItemId catalogueItemId) => taskModels.TryGetValue(catalogueItemId, out TaskListOrderItemModel value) ? value : null;
    }
}
