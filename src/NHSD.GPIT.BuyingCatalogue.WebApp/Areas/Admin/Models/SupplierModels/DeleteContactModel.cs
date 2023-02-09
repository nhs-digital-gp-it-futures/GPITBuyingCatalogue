using System;
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
            if (supplierContact is null)
                throw new ArgumentNullException(nameof(supplierContact));

            SupplierId = supplierContact.SupplierId;
            ContactId = supplierContact.Id;
            FirstName = supplierContact.FirstName;
            LastName = supplierContact.LastName;
            SupplierName = supplierName;
        }

        public int SupplierId { get; init; }

        public int ContactId { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string SupplierName { get; init; }
    }
}
