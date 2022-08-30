using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SelectSupplierModel : OrderingBaseModel
    {
        public CallOffId CallOffId { get; set; }

        public string SelectedSupplierId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public IEnumerable<SelectListItem> Suppliers { get; set; }
    }
}
