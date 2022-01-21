using System.Threading;
using System.Threading.Tasks;
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
                .MustAsync(NotBeADuplicateSupplierName)
                .WithMessage("Supplier name already exists. Enter a different name")
                .OverridePropertyName(m => m.SupplierName);

            RuleFor(m => m)
                .MustAsync(NotBeADuplicateSupplierLegalName)
                .WithMessage("Supplier legal name already exists. Enter a different name")
                .OverridePropertyName(m => m.SupplierLegalName);
        }

        private async Task<bool> NotBeADuplicateSupplierName(EditSupplierDetailsModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            var supplier = await suppliersService.GetSupplierByName(model.SupplierName);
            if (supplier is null)
                return true;

            return supplier.Id == model.SupplierId;
        }

        private async Task<bool> NotBeADuplicateSupplierLegalName(EditSupplierDetailsModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            var supplier = await suppliersService.GetSupplierByLegalName(model.SupplierLegalName);
            if (supplier is null)
                return true;

            return supplier.Id == model.SupplierId;
        }
    }
}
