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
        public const string AmendmentAdvice = "Add Service Recipients to existing items or add Additional Services.";
        public const string AmendmentTitle = "Amend items from the original order";
        public const string CompletedAdvice = "Select the sections that you want to edit.";
        public const string CompletedTitle = "Edit solutions and services";
        public const string InProgressAdvice = "Review the progress of your order. Make sure you’ve included everything you want to order and that all sections are completed.";
        public const string InProgressTitle = "Review your progress";

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
            var order = wrapper?.RolledUp;

            if (order == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            var previous = wrapper.Previous;

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            AssociatedServicesOnly = order.AssociatedServicesOnly;
            CatalogueSolution = order.GetSolution();
            AdditionalServices = order.GetAdditionalServices();
            AssociatedServices = order.GetAssociatedServices();

            if (order.AssociatedServicesOnly)
            {
                SolutionName = order.Solution?.Name;
            }

            if (CatalogueSolution != null)
            {
                taskModels.Add(CatalogueSolution.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, CatalogueSolution)
                {
                    FromPreviousRevision = previous?.OrderItems.Any(x => x.CatalogueItemId == CatalogueSolution.CatalogueItemId) ?? false,
                    HasCurrentAmendments = wrapper.HasCurrentAmendments(CatalogueSolution),
                    NumberOfPrices = CatalogueSolution.CatalogueItem.CataloguePrices.Count,
                    PriceId = CatalogueSolution.CatalogueItem.CataloguePrices.Count == 1
                        ? CatalogueSolution.CatalogueItem.CataloguePrices.First().CataloguePriceId
                        : 0,
                });
            }

            AdditionalServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, x)
            {
                FromPreviousRevision = previous?.OrderItems.Any(oi => oi.CatalogueItemId == x.CatalogueItemId) ?? false,
                HasCurrentAmendments = wrapper.HasCurrentAmendments(x),
                NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                    ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
            }));

            AssociatedServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, x)
            {
                FromPreviousRevision = previous?.OrderItems.Any(oi => oi.CatalogueItemId == x.CatalogueItemId) ?? false,
                HasCurrentAmendments = wrapper.HasCurrentAmendments(x),
                NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                    ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
            }));
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool AssociatedServicesOnly { get; set; }

        public override string Title => IsAmendment
            ? AmendmentTitle
            : Progress == TaskProgress.Completed ? CompletedTitle : InProgressTitle;

        public override string Caption => $"Order {CallOffId}";

        public override string Advice => IsAmendment
            ? AmendmentAdvice
            : Progress == TaskProgress.Completed ? CompletedAdvice : InProgressAdvice;

        public string SolutionName { get; set; }

        public OrderItem CatalogueSolution { get; set; }

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
