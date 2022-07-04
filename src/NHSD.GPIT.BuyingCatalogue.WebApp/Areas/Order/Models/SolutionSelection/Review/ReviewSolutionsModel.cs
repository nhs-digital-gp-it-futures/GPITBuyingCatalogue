using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Review
{
    public class ReviewSolutionsModel : NavBaseModel
    {
        public ReviewSolutionsModel()
        {
        }

        public ReviewSolutionsModel(EntityFramework.Ordering.Models.Order order, string internalOrgId)
        {
            CallOffId = order.CallOffId;
            InternalOrgId = internalOrgId;
            Order = order;
            AllOrderItems = order.OrderItems;
            CatalogueSolution = order.GetSolution();
            AdditionalServices = order.GetAdditionalServices().ToList();
            AssociatedServices = order.GetAssociatedServices().ToList();
            ContractLength = order.MaximumTerm ?? 0;
            AssociatedServicesOnly = order.AssociatedServicesOnly;
        }

        public CallOffId CallOffId { get; set; }

        public EntityFramework.Ordering.Models.Order Order { get; set; }

        public OrderItem CatalogueSolution { get; set; }

        public List<OrderItem> AdditionalServices { get; set; }

        public List<OrderItem> AssociatedServices { get; set; }

        public ICollection<OrderItem> AllOrderItems { get; set; }

        public int ContractLength { get; set; }

        public string InternalOrgId { get; set; }

        public bool AssociatedServicesOnly { get; set; }
    }
}
