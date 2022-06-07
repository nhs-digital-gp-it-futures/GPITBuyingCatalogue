using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers
{
    public class EditSupplierDetailsModelValidator : AbstractValidator<EditSupplierDetailsModel>
    {
        private readonly ISuppliersService suppliersService;

        public EditSupplierDetailsModelValidator(
            IUrlValidator urlValidator,
            ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m.SupplierName)
                .NotEmpty()
                .WithMessage("Enter a supplier name");

            RuleFor(m => m.SupplierLegalName)
                .NotEmpty()
                .WithMessage("Enter a supplier legal name");

            RuleFor(m => m.SupplierWebsite)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.SupplierWebsite));

            RuleFor(m => m)
                .Must(NotBeADuplicateSupplierName)
                .WithMessage("Supplier name already exists. Enter a different name")
                .OverridePropertyName(m => m.SupplierName);

            RuleFor(m => m)
                .Must(NotBeADuplicateSupplierLegalName)
                .WithMessage("Supplier legal name already exists. Enter a different name")
                .OverridePropertyName(m => m.SupplierLegalName);
        }

        private bool NotBeADuplicateSupplierName(EditSupplierDetailsModel model)
        {
            var supplier = suppliersService.GetSupplierByName(model.SupplierName).GetAwaiter().GetResult();
            if (supplier is null)
                return true;

            return supplier.Id == model.SupplierId;
        }

        private bool NotBeADuplicateSupplierLegalName(EditSupplierDetailsModel model)
        {
            var supplier = suppliersService.GetSupplierByLegalName(model.SupplierLegalName).GetAwaiter().GetResult();
            if (supplier is null)
                return true;

            return supplier.Id == model.SupplierId;
        }
    }
}
