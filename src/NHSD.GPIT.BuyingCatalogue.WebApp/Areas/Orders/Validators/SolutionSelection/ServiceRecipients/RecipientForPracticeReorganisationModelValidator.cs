using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients
{
    public class RecipientForPracticeReorganisationModelValidator : AbstractValidator<RecipientForPracticeReorganisationModel>
    {
        internal const string ErrorMessage = "Select a Service Recipient";

        public RecipientForPracticeReorganisationModelValidator()
        {
            RuleFor(m => m.SelectedOdsCode)
                .NotNull()
                .WithMessage(ErrorMessage);
        }
    }
}
