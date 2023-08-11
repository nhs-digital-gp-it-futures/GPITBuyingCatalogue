using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement
{
    public class RequirementDetailsModel : NavBaseModel
    {
        public RequirementDetailsModel()
        {
            AssociatedServices = Enumerable.Empty<OrderItem>();
        }

        public RequirementDetailsModel(CallOffId callOffId, string internalOrgId, IEnumerable<OrderItem> associatedServices)
        : this()
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            AssociatedServices = associatedServices ?? Enumerable.Empty<OrderItem>();
        }

        public RequirementDetailsModel(EntityFramework.Ordering.Models.Requirement item, CallOffId callOffId, string internalOrgId, IEnumerable<OrderItem> associatedServices)
            : this(callOffId, internalOrgId, associatedServices)
        {
            ItemId = item.Id;
            SelectedOrderItemId = item.OrderItem.CatalogueItemId;
            Details = item.Details;
        }

        public bool IsEdit => ItemId != 0;

        public int ItemId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public CatalogueItemId SelectedOrderItemId { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        public override string Advice => IsEdit ? "Edit Associated Service requirement." : "Add Associated Service requirement.";

        public IEnumerable<SelectOption<string>> OrderItemOptions => AssociatedServices.Select(x =>
            new SelectOption<string>(x.CatalogueItem.Name, x.CatalogueItem.Id.ToString())).ToList();

        public IEnumerable<OrderItem> AssociatedServices { get; set; }
    }
}
