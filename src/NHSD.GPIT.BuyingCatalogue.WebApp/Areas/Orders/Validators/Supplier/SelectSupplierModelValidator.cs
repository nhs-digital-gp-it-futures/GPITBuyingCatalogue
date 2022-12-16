using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier
{
    public class SelectSupplierModelValidator : AbstractValidator<SelectSupplierModel>
    {
        public const string SupplierMissingErrorMessage = "Enter a supplier name";

        public SelectSupplierModelValidator()
        {
            RuleFor(x => x.SelectedSupplierId)
                .NotEmpty()
                .WithMessage(SupplierMissingErrorMessage);
        }
    }
}
