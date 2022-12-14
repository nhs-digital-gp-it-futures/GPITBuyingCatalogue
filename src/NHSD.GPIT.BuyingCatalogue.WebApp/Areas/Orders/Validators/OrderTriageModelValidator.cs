using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators
{
    public class OrderTriageModelValidator : AbstractValidator<OrderTriageModel>
    {
        public OrderTriageModelValidator()
        {
            RuleFor(m => m.SelectedOrderTriageValue)
                .NotNull()
                .WithMessage("Select the approximate value of your order, or ‘I’m not sure’ if you do not know");
        }
    }
}
