using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    public class MatchDatesModelValidator : AbstractValidator<MatchDatesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select yes if you want to use the same delivery dates";

        public MatchDatesModelValidator()
        {
            RuleFor(x => x.MatchDates)
                .NotNull()
                .WithMessage(NoSelectionMadeErrorMessage);
        }
    }
}
