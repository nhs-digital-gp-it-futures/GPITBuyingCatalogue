using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling
{
    public class ContractBillingItemModel : NavBaseModel
    {
        public ContractBillingItemModel()
        {
            AssociatedServices = Enumerable.Empty<OrderItem>();
        }

        public ContractBillingItemModel(CallOffId callOffId, string internalOrgId, IEnumerable<OrderItem> associatedServices)
        : this()
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            AssociatedServices = associatedServices ?? Enumerable.Empty<OrderItem>();
        }

        public ContractBillingItemModel(ContractBillingItem item, CallOffId callOffId, string internalOrgId, IEnumerable<OrderItem> associatedServices)
            : this(callOffId, internalOrgId, associatedServices)
        {
            ItemId = item.Id;
            SelectedOrderItemId = item.OrderItem.CatalogueItemId;
            Name = item.Milestone?.Title;
            PaymentTrigger = item.Milestone?.PaymentTrigger;
            Quantity = item.Quantity;
        }

        public bool IsEdit => ItemId != 0;

        public int ItemId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public CatalogueItemId SelectedOrderItemId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string PaymentTrigger { get; set; }

        [Description("Quantity")]
        [ModelBinder(typeof(NumberModelBinder))]
        public int? Quantity { get; set; }

        public override string Advice => IsEdit ? "Edit this Associated Service milestone." : "Add an Associated Service milestone.";

        public IEnumerable<SelectOption<string>> OrderItemOptions => AssociatedServices.Select(x =>
            new SelectOption<string>(x.CatalogueItem.Name, x.CatalogueItem.Id.ToString())).ToList();

        public IEnumerable<OrderItem> AssociatedServices { get; set; }
    }
}
