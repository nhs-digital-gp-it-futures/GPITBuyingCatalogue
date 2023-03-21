using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review
{
    public class ReviewSolutionsModel : NavBaseModel
    {
        public ReviewSolutionsModel(OrderWrapper wrapper, string internalOrgId)
        {
            OrderWrapper = wrapper;
            InternalOrgId = internalOrgId;
        }

        public OrderWrapper OrderWrapper { get; }

        public CallOffId CallOffId => OrderWrapper.Order.CallOffId;

        public Order Order => OrderWrapper.Order;

        public Order Previous => OrderWrapper.Previous;

        public Order RolledUp => OrderWrapper.RolledUp;

        public List<OrderItem> CatalogueSolutions => RolledUp.GetSolutions().ToList();

        public List<OrderItem> AdditionalServices => RolledUp.GetAdditionalServices().ToList();

        public List<OrderItem> AssociatedServices => RolledUp.GetAssociatedServices().ToList();

        public ICollection<OrderItem> AllOrderItems => OrderWrapper.Order.OrderItems;

        public int ContractLength => OrderWrapper.Order.MaximumTerm ?? 0;

        public string InternalOrgId { get; set; }

        public bool AssociatedServicesOnly => OrderWrapper.Order.AssociatedServicesOnly;
    }
}
