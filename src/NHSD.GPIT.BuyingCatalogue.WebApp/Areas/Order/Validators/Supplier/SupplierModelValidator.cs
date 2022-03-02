using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier
{
    public class SupplierModelValidator : AbstractValidator<SupplierModel>
    {
        public const string ContactNotSelectedErrorMessage = "Select a contact for this supplier";

        public SupplierModelValidator()
        {
            RuleFor(x => x.SelectedContactId)
                .NotNull()
                .WithMessage(ContactNotSelectedErrorMessage);
        }
    }
}
