using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class EditContactModel : NavBaseModel
    {
        public EditContactModel()
        {
        }

        public EditContactModel(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            Title = "Add a contact";
            SupplierId = supplier.Id;
            SupplierName = supplier.Name;
        }

        public EditContactModel(SupplierContact contact, Supplier supplier)
        {
            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            ContactId = contact.Id;
            SupplierId = contact.SupplierId;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Email = contact.Email;
            PhoneNumber = contact.PhoneNumber;
            Department = contact.Department;

            Title = $"{FirstName} {LastName} contact details";
            SupplierName = supplier.Name;
        }

        public int? ContactId { get; init; }

        public int SupplierId { get; init; }

        [StringLength(35)]
        public string FirstName { get; init; }

        [StringLength(35)]
        public string LastName { get; init; }

        [StringLength(255)]
        public string Email { get; init; }

        [StringLength(35)]
        public string PhoneNumber { get; init; }

        [StringLength(50)]
        public string Department { get; init; }

        public string Title { get; init; }

        public string SupplierName { get; init; }
    }
}
