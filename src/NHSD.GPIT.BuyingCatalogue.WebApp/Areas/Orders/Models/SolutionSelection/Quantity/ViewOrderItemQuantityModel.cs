using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity
{
    public class ViewOrderItemQuantityModel : NavBaseModel
    {
        public const string AdviceText = "These are the quantities you agreed in the original contract. You are unable to change the originally agreed quantities.";
        public const string TitleText = "Quantity of {0}";

        public ViewOrderItemQuantityModel(OrderItem orderItem)
        {
            if (orderItem == null)
            {
                throw new ArgumentNullException(nameof(orderItem));
            }

            ItemName = orderItem.CatalogueItem.Name;
            ItemType = orderItem.CatalogueItem.CatalogueItemType.Name();
            Quantity = orderItem.Quantity.HasValue ? $"{orderItem.Quantity:N0}" : string.Empty;
            QuantityDescription = orderItem.OrderItemPrice?.RangeDescription;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public override string Title => string.Format(TitleText, ItemType);

        public override string Caption => ItemName;

        public override string Advice => AdviceText;

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public string Quantity { get; set; }

        public string QuantityDescription { get; set; }
    }
}
