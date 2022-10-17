using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.DeliveryDates
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
