using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class EditContactModel : ContactModel
    {
        public const string AddContactAdvice = "Provide the following details to add a new contact for this supplier.";
        public const string EditContactAdvice = "Provide details for this contact for {0}.";

        public EditContactModel()
        {
        }

        public EditContactModel(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            SupplierId = supplier.Id;

            Caption = supplier.Name;
            Advice = AddContactAdvice;
        }

        public EditContactModel(SupplierContact contact, Supplier supplier, IList<CatalogueItem> solutionsReferencing)
        {
            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            SolutionsReferencingThisContact = solutionsReferencing ?? throw new ArgumentNullException(nameof(solutionsReferencing));

            ContactId = contact.Id;
            SupplierId = contact.SupplierId;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Email = contact.Email;
            PhoneNumber = contact.PhoneNumber;
            Department = contact.Department;

            Caption = contact.NameOrDepartment;
            Advice = string.Format(EditContactAdvice, supplier.Name);
        }

        public int? ContactId { get; init; }

        public int SupplierId { get; init; }

        public string Caption { get; init; }

        public string Advice { get; init; }

        public IList<CatalogueItem> SolutionsReferencingThisContact { get; set; }
    }
}
