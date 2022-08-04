using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditContactModelValidator : AbstractValidator<EditContactModel>
    {
        public const string ContactDetailsMissingErrorMessage = "Enter a telephone number or email address";
        public const string DuplicateContactErrorMessage = "A contact with these contact details already exists for this supplier";
        public const string EmailAddressFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string FirstNameMissingErrorMessage = "Enter a first name";
        public const string LastNameMissingErrorMessage = "Enter a last name";
        public const string PersonalDetailsMissingErrorMessage = "Enter a contact name or department name";

        private readonly ISuppliersService suppliersService;

        public EditContactModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(x => x)
                .Must(HaveEnteredPersonalDetails)
                .WithMessage(PersonalDetailsMissingErrorMessage)
                .OverridePropertyName(x => x.FirstName);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameMissingErrorMessage)
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(LastNameMissingErrorMessage)
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x)
                .Must(HaveEnteredContactDetails)
                .WithMessage(ContactDetailsMissingErrorMessage)
                .OverridePropertyName(x => x.PhoneNumber);

            RuleFor(m => m.Email)
                .EmailAddress()
                .WithMessage(EmailAddressFormatErrorMessage)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(m => m)
                .Must(NotBeADuplicateContact)
                .WithMessage(DuplicateContactErrorMessage)
                .OverridePropertyName("edit-contact");
        }

        private static bool HaveEnteredContactDetails(EditContactModel model)
        {
            return !string.IsNullOrWhiteSpace(model.PhoneNumber)
                || !string.IsNullOrWhiteSpace(model.Email);
        }

        private static bool HaveEnteredPersonalDetails(EditContactModel model)
        {
            return !string.IsNullOrWhiteSpace(model.FirstName)
                || !string.IsNullOrWhiteSpace(model.LastName)
                || !string.IsNullOrWhiteSpace(model.Department);
        }

        private bool NotBeADuplicateContact(EditContactModel model)
        {
            var supplier = suppliersService.GetSupplier(model.SupplierId).GetAwaiter().GetResult();

            return !supplier.SupplierContacts
                .Where(sc => sc.Id != model.ContactId)
                .Any(sc => string.Equals(sc.FirstName?.Trim(), model.FirstName?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.LastName?.Trim(), model.LastName?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.Email?.Trim(), model.Email?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.PhoneNumber?.Trim(), model.PhoneNumber?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(sc.Department?.Trim(), model.Department?.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
