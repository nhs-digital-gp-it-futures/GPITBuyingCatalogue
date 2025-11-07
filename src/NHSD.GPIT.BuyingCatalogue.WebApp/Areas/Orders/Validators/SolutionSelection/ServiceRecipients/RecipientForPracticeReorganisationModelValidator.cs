using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients
{
    public class RecipientForPracticeReorganisationModelValidator : AbstractValidator<RecipientForPracticeReorganisationModel>
    {
        internal const string ErrorMessage = "Select a service recipient";

        public RecipientForPracticeReorganisationModelValidator()
        {
            RuleFor(m => m.SelectedRecipientId)
                .NotNull()
                .WithMessage(ErrorMessage);
        }
    }
}
