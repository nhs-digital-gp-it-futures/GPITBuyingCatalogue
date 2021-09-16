using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class ManageSuppliersModel : NavBaseModel
    {
        public ManageSuppliersModel()
        {
        }

        public ManageSuppliersModel(List<Supplier> suppliers)
        {
            Suppliers = suppliers;
        }

        public bool ShowInactiveSuppliers { get; } // Only used for the checkbox component. value is not actually used server side;

        public IReadOnlyList<Supplier> Suppliers { get; set; }
    }
}
