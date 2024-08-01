using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier
{
    public class ConfirmSupplierModelValidator : AbstractValidator<ConfirmSupplierModel>
    {
        public const string ConfirmSupplierErrorMessage = "Select yes if you want to continue with this supplier";

        public ConfirmSupplierModelValidator()
        {
            RuleFor(x => x.ConfirmSupplier)
                .NotNull()
                .Equal(true)
                .WithMessage(ConfirmSupplierErrorMessage);
        }
    }
}
