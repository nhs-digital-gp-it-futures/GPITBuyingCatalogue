using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListModel : NavBaseModel
    {
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
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public OrderItem CatalogueSolution { get; set; }

        public IEnumerable<OrderItem> AdditionalServices { get; set; }

        public IEnumerable<OrderItem> AssociatedServices { get; set; }
    }
}
