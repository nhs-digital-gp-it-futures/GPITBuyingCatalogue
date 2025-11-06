using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients
{
    public class RemoveSublocationsModelValidator : AbstractValidator<RemoveSublocationsModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select whether to remove or keep your selected {0}";

        public RemoveSublocationsModelValidator()
        {
            RuleFor(x => x.ConfirmRemove)
                .NotNull()
                .WithMessage(x => string.Format(
                    NoSelectionMadeErrorMessage,
                    x.Pluralisation))
                .OverridePropertyName(nameof(RemoveSublocationsModel.ConfirmRemove));
        }
    }
}
