using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class DeleteOrderModelValidator : AbstractValidator<DeleteOrderModel>
    {
        public const string SelectOptionErrorMessage = "Select yes if you want to delete this order";

        public DeleteOrderModelValidator()
        {
            RuleFor(m => m.SelectedOption)
                .NotNull()
                .WithMessage(SelectOptionErrorMessage);
        }
    }
}
