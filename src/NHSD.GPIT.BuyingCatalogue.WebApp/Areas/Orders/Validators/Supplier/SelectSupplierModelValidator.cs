using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier
{
    public class SelectSupplierModelValidator : AbstractValidator<SelectSupplierModel>
    {
        public const string SupplierSearchMissingErrorMessage = "Enter a supplier name";
        public const string SupplierSelectMissingErrorMessage = "Select a supplier";

        public SelectSupplierModelValidator()
        {
            RuleFor(x => x.SelectedSupplierId)
                .NotEmpty()
                .WithMessage(SupplierSearchMissingErrorMessage)
                .When(x => x.OrderType.UsesSupplierSearch);

            RuleFor(x => x.SelectedSupplierId)
                .NotEmpty()
                .WithMessage(SupplierSelectMissingErrorMessage)
                .When(x => !x.OrderType.UsesSupplierSearch)
                .OverridePropertyName("selected-supplier-id");
        }
    }
}
