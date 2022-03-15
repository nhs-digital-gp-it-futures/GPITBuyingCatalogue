using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public class ViewOrderModel : NavBaseModel
    {
        public ViewOrderModel(EntityFramework.Ordering.Models.Order order)
        {
            var solutionOrderItem = order.OrderItems?.FirstOrDefault(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
            var framework = solutionOrderItem?.CatalogueItem?.Solution?.FrameworkSolutions?.FirstOrDefault(fs => fs.FrameworkId != "COVID");

            CallOffId = order.CallOffId;
            Description = order.Description;
            LastUpdatedBy = order.LastUpdatedByUser.FullName;
            OrganisationName = order.OrderingParty.Name;
            OrganisationExternalIdentifier = order.OrderingParty.ExternalIdentifier;
            OrganisationInternalIdentifier = order.OrderingParty.InternalIdentifier;
            SupplierName = order?.Supplier?.Name;
            OrderStatus = order.OrderStatus;
            OrderItems = order.OrderItems?.Select(oi => new AdminViewOrderItem
            {
                Name = oi.CatalogueItem.Name,
                Type = oi.CatalogueItem.CatalogueItemType,
                FundingSourceOnlyGms = order.FundingSourceOnlyGms,
                ConfirmedFundingSource = order.ConfirmedFundingSource,
                Framework = framework?.Framework?.ShortName ?? string.Empty,
            });
        }

        public CallOffId CallOffId { get; set; }

        public string OrganisationName { get; set; }

        public string OrganisationInternalIdentifier { get; set; }

        public string OrganisationExternalIdentifier { get; set; }

        public string Description { get; set; }

        public string LastUpdatedBy { get; set; }

        public string SupplierName { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public IEnumerable<AdminViewOrderItem> OrderItems { get; set; }
    }
}
