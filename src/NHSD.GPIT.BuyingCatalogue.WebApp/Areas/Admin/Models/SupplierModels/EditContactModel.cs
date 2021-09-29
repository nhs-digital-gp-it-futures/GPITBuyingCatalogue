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
            Title = "Add a contact";
            SupplierName = supplier.Name;
        }

        public EditContactModel(SupplierContact contact, Supplier supplier)
        {
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

        public int ContactId { get; }

        public int SupplierId { get; }

        [StringLength(35)]
        [Required(ErrorMessage = "Enter a first name")]
        public string FirstName { get; init; }

        [StringLength(35)]
        [Required(ErrorMessage = "Enter a last name")]
        public string LastName { get; init; }

        [StringLength(255)]
        [Required(ErrorMessage = "Enter an email address")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        public string Email { get; init; }

        [StringLength(35)]
        [Required(ErrorMessage = "Enter a phone number")]
        public string PhoneNumber { get; init; }

        [StringLength(50)]
        [Required(ErrorMessage = "Enter a department name")]
        public string Department { get; init; }

        public string Title { get; }

        public string SupplierName { get; }
    }
}
