using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class SupplierSearchSelectModel : OrderingBaseModel
    {
        public SupplierSearchSelectModel(string odsCode, CallOffId callOffId, List<EntityFramework.Catalogue.Models.Supplier> suppliers)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/supplier/search";
            Title = "Suppliers found";
            OdsCode = odsCode;
            Suppliers = suppliers;
        }

        public SupplierSearchSelectModel()
        {
        }

        public List<EntityFramework.Catalogue.Models.Supplier> Suppliers { get; set; }

        [Required(ErrorMessage = "Please select a supplier")]
        public int? SelectedSupplierId { get; set; }
    }
}
