using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.DeleteOrder
{
    public class DeleteOrderModelValidator : AbstractValidator<DeleteOrderModel>
    {
        public const string AmendmentNoSelectionMadeErrorMessage = "Select yes if you want to delete this amendment";
        public const string OrderNoSelectionMadeErrorMessage = "Select yes if you want to delete this order";

        public DeleteOrderModelValidator()
        {
            RuleFor(m => m.SelectedOption)
                .NotNull()
                .When(x => x.IsAmendment)
                .WithMessage(AmendmentNoSelectionMadeErrorMessage);

            RuleFor(m => m.SelectedOption)
                .NotNull()
                .When(x => !x.IsAmendment)
                .WithMessage(OrderNoSelectionMadeErrorMessage);
        }
    }
}
