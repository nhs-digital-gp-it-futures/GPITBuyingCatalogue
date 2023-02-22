using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review
{
    public class ReviewSolutionsModel : NavBaseModel
    {
        public ReviewSolutionsModel()
        {
        }

        public ReviewSolutionsModel(OrderWrapper wrapper, string internalOrgId)
        {
            var order = wrapper.Order;

            CallOffId = order.CallOffId;
            InternalOrgId = internalOrgId;
            Order = order;
            Previous = wrapper.Previous;
            RolledUp = wrapper.RolledUp;
            AllOrderItems = order.OrderItems;
            CatalogueSolutions = RolledUp.GetSolutions().ToList();
            AdditionalServices = RolledUp.GetAdditionalServices().ToList();
            AssociatedServices = RolledUp.GetAssociatedServices().ToList();
            ContractLength = order.MaximumTerm ?? 0;
            AssociatedServicesOnly = order.AssociatedServicesOnly;
        }

        public CallOffId CallOffId { get; set; }

        public Order Order { get; set; }

        public Order Previous { get; set; }

        public Order RolledUp { get; set; }

        public List<OrderItem> CatalogueSolutions { get; set; }

        public List<OrderItem> AdditionalServices { get; set; }

        public List<OrderItem> AssociatedServices { get; set; }

        public ICollection<OrderItem> AllOrderItems { get; set; }

        public int ContractLength { get; set; }

        public string InternalOrgId { get; set; }

        public bool AssociatedServicesOnly { get; set; }
    }
}
