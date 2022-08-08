using System;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditContactModelValidator : AbstractValidator<EditContactModel>
    {
        public const string DuplicateContactErrorMessage = "A contact with these contact details already exists for this supplier";

        private readonly ISuppliersService suppliersService;

        public EditContactModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            Include(new ContactModelValidator());

            RuleFor(m => m)
                .Must(NotBeADuplicateContact)
                .WithMessage(DuplicateContactErrorMessage)
                .OverridePropertyName("edit-contact");
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
