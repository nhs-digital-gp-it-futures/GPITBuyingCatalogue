using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SupplierSearchSelectModel : OrderingBaseModel
    {
        public SupplierSearchSelectModel(string odsCode, string callOffId, List<EntityFramework.Models.GPITBuyingCatalogue.Supplier> suppliers)
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

        public List<EntityFramework.Models.GPITBuyingCatalogue.Supplier> Suppliers { get; init; }

        [Required]
        public string SelectedSupplierId { get; set; }
    }
}
