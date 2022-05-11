using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            AllOrderItems = order.OrderItems;
            CatalogueSolution = order.OrderItems.SingleOrDefault(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
            AdditionalServices = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService).ToList();
            AssociatedServices = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService).ToList();
            ContractLength = order.MaximumTerm.Value;
        }

        public CallOffId CallOffId { get; set; }

        public OrderItem CatalogueSolution { get; set; }

        public List<OrderItem> AdditionalServices { get; set; }

        public List<OrderItem> AssociatedServices { get; set; }

        public ICollection<OrderItem> AllOrderItems { get; set; }

        public int ContractLength { get; set; }

        public string InternalOrgId { get; set; }
    }
}
