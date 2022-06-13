using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListModel : NavBaseModel
    {
        private readonly Dictionary<CatalogueItemId, TaskListOrderItemModel> taskModels = new();

        public TaskListModel()
        {
        }

        public TaskListModel(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

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
                    NumberOfPrices = CatalogueSolution.CatalogueItem.CataloguePrices.Count,
                    PriceId = CatalogueSolution.CatalogueItem.CataloguePrices.Count == 1
                        ? CatalogueSolution.CatalogueItem.CataloguePrices.First().CataloguePriceId
                        : 0,
                });
            }

            AdditionalServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, x)
            {
                NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                    ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
            }));

            AssociatedServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, x)
            {
                NumberOfPrices = x.CatalogueItem.CataloguePrices.Count,
                PriceId = x.CatalogueItem.CataloguePrices.Count == 1
                    ? x.CatalogueItem.CataloguePrices.First().CataloguePriceId
                    : 0,
            }));
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public string SolutionName { get; set; }

        public OrderItem CatalogueSolution { get; set; }

        public bool AdditionalServicesAvailable { get; set; }

        public IEnumerable<OrderItem> AdditionalServices { get; set; }

        public bool AssociatedServicesAvailable { get; set; }

        public IEnumerable<OrderItem> AssociatedServices { get; set; }

        public TaskProgress Progress
        {
            get
            {
                if (taskModels.All(x => x.Value.ServiceRecipientsStatus == TaskProgress.Completed)
                    && taskModels.All(x => x.Value.PriceStatus == TaskProgress.Completed)
                    && taskModels.All(x => x.Value.QuantityStatus == TaskProgress.Completed))
                {
                    return TaskProgress.Completed;
                }

                return TaskProgress.InProgress;
            }
        }

        public TaskListOrderItemModel OrderItemModel(CatalogueItemId catalogueItemId) => taskModels.ContainsKey(catalogueItemId)
            ? taskModels[catalogueItemId]
            : null;
    }
}
