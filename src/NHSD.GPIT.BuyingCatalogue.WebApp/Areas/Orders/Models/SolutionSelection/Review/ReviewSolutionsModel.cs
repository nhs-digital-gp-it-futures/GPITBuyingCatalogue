using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review
{
    public class ReviewSolutionsModel : NavBaseModel
    {
        public ReviewSolutionsModel()
        {
        }

        public ReviewSolutionsModel(Order order, string internalOrgId)
        {
            CallOffId = order.CallOffId;
            InternalOrgId = internalOrgId;
            Order = order;
            AllOrderItems = order.OrderItems;
            CatalogueSolutions = order.GetSolutions().ToList();
            AdditionalServices = order.GetAdditionalServices().ToList();
            AssociatedServices = order.GetAssociatedServices().ToList();
            ContractLength = order.MaximumTerm ?? 0;
            AssociatedServicesOnly = order.AssociatedServicesOnly;
        }

        public CallOffId CallOffId { get; set; }

        public Order Order { get; set; }

        public List<OrderItem> CatalogueSolutions { get; set; }

        public List<OrderItem> AdditionalServices { get; set; }

        public List<OrderItem> AssociatedServices { get; set; }

        public ICollection<OrderItem> AllOrderItems { get; set; }

        public int ContractLength { get; set; }

        public string InternalOrgId { get; set; }

        public bool AssociatedServicesOnly { get; set; }
    }
}
