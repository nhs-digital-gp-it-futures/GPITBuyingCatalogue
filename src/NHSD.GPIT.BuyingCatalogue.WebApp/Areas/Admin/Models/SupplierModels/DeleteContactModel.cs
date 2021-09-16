using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class DeleteContactModel : NavBaseModel
    {
        public DeleteContactModel()
        {
        }

        public DeleteContactModel(SupplierContact supplierContact, string supplierName)
        {
            SupplierId = supplierContact.SupplierId;
            ContactId = supplierContact.Id;
            FirstName = supplierContact.FirstName;
            LastName = supplierContact.LastName;
            SupplierName = supplierName;
        }

        public int SupplierId { get; }

        public int ContactId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string SupplierName { get; }
    }
}
