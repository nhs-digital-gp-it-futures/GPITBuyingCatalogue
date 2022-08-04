using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public class TriageDueDiligenceModelValidator : AbstractValidator<TriageDueDiligenceModel>
    {
        public const string NoSelectionErrorMessage = "Select yes if you’ve identified what you want to order";

        public TriageDueDiligenceModelValidator()
        {
            RuleFor(m => m.Selected)
                .NotNull()
                .WithMessage(NoSelectionErrorMessage);
        }
    }
}
