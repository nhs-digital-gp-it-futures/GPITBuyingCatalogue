using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier
{
    public class SelectSupplierModelValidator : AbstractValidator<SelectSupplierModel>
    {
        public const string SupplierMissingErrorMessage = "Select a supplier";

        public SelectSupplierModelValidator()
        {
            RuleFor(x => x.SelectedSupplierId)
                .NotEmpty()
                .WithMessage(SupplierMissingErrorMessage);
        }
    }
}
