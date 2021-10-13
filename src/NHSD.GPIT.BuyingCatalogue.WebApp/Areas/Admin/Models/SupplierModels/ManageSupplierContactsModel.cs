using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class ManageSupplierContactsModel : NavBaseModel
    {
        public ManageSupplierContactsModel()
        {
        }

        public ManageSupplierContactsModel(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            SupplierName = supplier.Name;
            SupplierId = supplier.Id;
            Contacts = supplier.SupplierContacts;
        }

        public string SupplierName { get; }

        public int SupplierId { get; }

        public ICollection<SupplierContact> Contacts { get; }
    }
}
