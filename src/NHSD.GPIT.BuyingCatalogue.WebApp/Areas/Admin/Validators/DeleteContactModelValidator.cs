using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class DeleteContactModelValidator : AbstractValidator<DeleteContactModel>
    {
        public const string LastContactOnSupplierErrorMessage = "Cannot delete last contact from active supplier";

        private readonly ISuppliersService suppliersService;

        public DeleteContactModelValidator(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService;

            RuleFor(m => m)
                .Must(NotBeLastContactOnActiveSupplier)
                .WithMessage(LastContactOnSupplierErrorMessage)
                .OverridePropertyName("delete-contact");
        }

        private bool NotBeLastContactOnActiveSupplier(DeleteContactModel model)
        {
            var supplier = suppliersService.GetSupplier(model.SupplierId).GetAwaiter().GetResult();
            return supplier.CanDeleteFromSupplierContacts();
        }
    }
}
