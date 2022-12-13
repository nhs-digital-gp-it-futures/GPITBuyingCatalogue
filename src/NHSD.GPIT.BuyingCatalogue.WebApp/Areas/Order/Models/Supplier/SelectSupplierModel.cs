using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SelectSupplierModel : OrderingBaseModel
    {
        public CallOffId CallOffId { get; set; }

        public string SelectedSupplierId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public IEnumerable<SelectOption<string>> Suppliers { get; set; }
    }
}
