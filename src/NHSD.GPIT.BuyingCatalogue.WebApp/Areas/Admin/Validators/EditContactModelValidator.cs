using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditContactModelValidator : AbstractValidator<EditContactModel>
    {
        public const string FirstNameError = "Enter a first name";
        public const string LastNameError = "Enter a last name";
        public const string PhoneNumberError = "Enter a phone number";
        public const string DepartmentNameError = "Enter a department name";
        public const string NoEmailError = "Enter an email address";
        public const string EmailFormatError = "Enter an email address in the correct format, like name@example.com";
        public const string ContactAlreadyExistsError = "A contact with these contact details already exists for this supplier";

        private readonly ISuppliersService suppliersService;

        public EditContactModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameError);

            RuleFor(m => m.LastName)
                .NotEmpty()
                .WithMessage(LastNameError);

            RuleFor(m => m.PhoneNumber)
                .NotEmpty()
                .WithMessage(PhoneNumberError);

            RuleFor(m => m.Department)
                .NotEmpty()
                .WithMessage(DepartmentNameError);

            RuleFor(m => m.Email)
                .NotEmpty()
                .WithMessage(NoEmailError)
                .EmailAddress()
                .WithMessage(EmailFormatError);

            RuleFor(m => m)
                .Must(NotBeADuplicateContact)
                .WithMessage(ContactAlreadyExistsError)
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
