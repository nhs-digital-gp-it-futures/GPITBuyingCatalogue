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

            if (CatalogueSolution != null)
            {
                taskModels.Add(CatalogueSolution.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, CatalogueSolution));
            }

            AdditionalServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, x)));
            AssociatedServices.ForEach(x => taskModels.Add(x.CatalogueItemId, new TaskListOrderItemModel(internalOrgId, callOffId, x)));
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public OrderItem CatalogueSolution { get; set; }

        public IEnumerable<OrderItem> AdditionalServices { get; set; }

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
