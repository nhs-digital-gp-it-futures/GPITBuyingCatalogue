using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers
{
    public class EditSupplierDetailsModelValidator : AbstractValidator<EditSupplierDetailsModel>
    {
        public const string EnterSupplierNameError = "Enter a supplier name";
        public const string EnterSupplierLegalNameError = "Enter a supplier legal name";
        public const string DuplicateSupplierNameError = "Supplier name already exists. Enter a different name";
        public const string DuplicateSupplierLegalNameError = "Supplier legal name already exists. Enter a different name";
        public const string EnterPrefixToUrlError = "Enter a prefix to the URL, either http or https";
        public const string EnterValidUrlError = "Enter a valid URL";

        private readonly ISuppliersService suppliersService;

        public EditSupplierDetailsModelValidator(
            IUrlValidator urlValidator,
            ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m.SupplierName)
                .NotEmpty()
                .WithMessage(EnterSupplierNameError);

            RuleFor(m => m.SupplierLegalName)
                .NotEmpty()
                .WithMessage(EnterSupplierLegalNameError);

            RuleFor(m => m.SupplierWebsite)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.SupplierWebsite));

            RuleFor(m => m)
                .Must(NotBeADuplicateSupplierName)
                .WithMessage(DuplicateSupplierNameError)
                .OverridePropertyName(m => m.SupplierName);

            RuleFor(m => m)
                .Must(NotBeADuplicateSupplierLegalName)
                .WithMessage(DuplicateSupplierLegalNameError)
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
