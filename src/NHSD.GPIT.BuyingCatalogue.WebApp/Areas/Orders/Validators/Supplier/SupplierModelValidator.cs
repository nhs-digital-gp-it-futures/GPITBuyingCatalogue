using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier
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
