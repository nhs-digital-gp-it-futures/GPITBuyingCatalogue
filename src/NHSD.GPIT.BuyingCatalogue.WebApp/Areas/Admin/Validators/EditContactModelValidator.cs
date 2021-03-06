using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditContactModelValidator : AbstractValidator<EditContactModel>
    {
        private readonly ISuppliersService suppliersService;

        public EditContactModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m.FirstName)
                .NotEmpty()
                .WithMessage("Enter a first name");

            RuleFor(m => m.LastName)
                .NotEmpty()
                .WithMessage("Enter a last name");

            RuleFor(m => m.PhoneNumber)
                .NotEmpty()
                .WithMessage("Enter a phone number");

            RuleFor(m => m.Department)
                .NotEmpty()
                .WithMessage("Enter a department name");

            RuleFor(m => m.Email)
                .NotEmpty()
                .WithMessage("Enter an email address")
                .EmailAddress()
                .WithMessage("Enter an email address in the correct format, like name@example.com");

            RuleFor(m => m)
                .Must(NotBeADuplicateContact)
                .WithMessage("A contact with these contact details already exists for this supplier")
                .OverridePropertyName("edit-contact");
        }

        private bool NotBeADuplicateContact(EditContactModel model)
        {
            var supplier = suppliersService.GetSupplier(model.SupplierId).GetAwaiter().GetResult();

            return !supplier.SupplierContacts
                .Where(sc => sc.Id != model.ContactId)
                .Any(sc => string.Equals(sc.FirstName.Trim(), model.FirstName?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.LastName.Trim(), model.LastName?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.Email.Trim(), model.Email?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.PhoneNumber.Trim(), model.PhoneNumber?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.Department.Trim(), model.Department?.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
