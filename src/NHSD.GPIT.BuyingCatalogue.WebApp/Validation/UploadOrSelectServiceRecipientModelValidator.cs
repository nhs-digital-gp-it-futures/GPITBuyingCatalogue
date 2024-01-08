using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class UploadOrSelectServiceRecipientModelValidator : AbstractValidator<UploadOrSelectServiceRecipientModel>
    {
        internal const string SelectedServiceRecipientOptionsError = "Select how you would like to add Service Recipients";

        public UploadOrSelectServiceRecipientModelValidator()
        {
            RuleFor(m => m.SelectedServiceRecipientOptions)
                .NotNull()
                .WithMessage(SelectedServiceRecipientOptionsError);
        }
    }
}
