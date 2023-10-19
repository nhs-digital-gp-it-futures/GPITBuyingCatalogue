using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier
{
    public class SelectSupplierModel : OrderingBaseModel
    {
        private const string TitleText = "Supplier information";

        private static readonly PageTitleModel StandardPageTitle = new()
        {
            Title = TitleText,
            Advice = "Search for the supplier you want to order from. You'll only be able to find suppliers with solutions published on the Buying Catalogue.",
        };

        private static readonly PageTitleModel SelectionPageTitle = new()
        {
            Title = TitleText,
            Advice = "You'll only be able to select suppliers that offer either mergers or splits as Additional Services with their solutions.",
        };

        public CallOffId CallOffId { get; set; }

        public string SelectedSupplierId { get; set; }

        public OrderType OrderType { get; set; }

        public IList<SelectOption<string>> Suppliers { get; set; } = new List<SelectOption<string>>();

        public PageTitleModel GetPageTitle()
        {
            if (OrderType.UsesSupplierSearch)
            {
                return StandardPageTitle with { Caption = $"Order {CallOffId}" };
            }
            else
            {
                return SelectionPageTitle with { Caption = $"Order {CallOffId}" };
            }
        }

        public string GetInsetText()
        {
            return OrderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceOther => "You’ll only be able to find suppliers that offer Associated Services with their solutions.",
                _ => string.Empty,
            };
        }
    }
}
