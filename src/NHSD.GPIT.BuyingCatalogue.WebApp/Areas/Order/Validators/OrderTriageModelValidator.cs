using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public class OrderTriageModelValidator : AbstractValidator<OrderTriageModel>
    {
        public const string ApproximateOrderValueErrorMessage = "Select the approximate value of your order, or ‘I’m not sure’ if you do not know";

        public OrderTriageModelValidator()
        {
            RuleFor(m => m.SelectedOrderTriageValue)
                .NotNull()
                .WithMessage(ApproximateOrderValueErrorMessage);
        }
    }
}
