using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListModel : NavBaseModel
    {
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
            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            AssociatedServicesOnly = wrapper.AssociatedServicesOnly;
            CatalogueSolution = wrapper.Solution;
            AdditionalServices = wrapper.AdditionalServices();
            AssociatedServices = wrapper.AssociatedServices();

            if (wrapper.AssociatedServicesOnly)
            {
                SolutionName = wrapper.Order.Solution?.Name;
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

        public string AdditionalServicesActionText => $"{(AdditionalServices?.Any() ?? false ? Change : Add)} Additional Services";

        public bool AssociatedServicesAvailable { get; set; }

        public IEnumerable<OrderItem> AssociatedServices { get; set; }

        public string AssociatedServicesActionText => $"{(AssociatedServices?.Any() ?? false ? Change : Add)} Associated Services";

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
