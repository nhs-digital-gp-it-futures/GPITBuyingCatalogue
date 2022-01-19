using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public sealed class SupplierSearchSelectModelValidator : AbstractValidator<SupplierSearchSelectModel>
    {
        public SupplierSearchSelectModelValidator()
        {
            RuleFor(m => m.SelectedSupplierId)
                .NotNull()
                .WithMessage("Please select a supplier");
        }
    }
}
