using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier
{
    public class ConfirmSupplierModel : OrderingBaseModel
    {
        private static readonly PageTitleModel StandardSupplierConfirmationPageTitle = new()
        {
            Title = "Supplier details",
            Advice = "Confirm this is the supplier you want to order from",
        };

        private static readonly PageTitleModel SingleSupplierConfirmationPageTitle = new()
        {
            Title = "Supplier information",
            Advice = "There is only one supplier that provides an Associated Service supporting mergers and splits.",
        };

        public ConfirmSupplierModel()
        {
        }

        public ConfirmSupplierModel(string internalOrgId, CallOffId callOffId, EntityFramework.Catalogue.Models.Supplier supplier, bool onlyOption = false)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            SupplierId = supplier.Id;
            Name = supplier.Name;
            LegalName = supplier.LegalName;
            Address = supplier.Address;
            OnlyOption = onlyOption;
        }

        public CallOffId CallOffId { get; set; }

        public bool OnlyOption { get; set; }

        public int SupplierId { get; set; }

        public string Name { get; set; }

        public string LegalName { get; set; }

        public Address Address { get; set; }

        public PageTitleModel GetPageTitle()
        {
            if (OnlyOption)
            {
                return SingleSupplierConfirmationPageTitle with { Caption = $"Order {CallOffId}" };
            }
            else
            {
                return StandardSupplierConfirmationPageTitle with { Caption = $"Order {CallOffId}" };
            }
        }
    }
}
