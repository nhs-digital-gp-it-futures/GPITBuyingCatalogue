using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients
{
    public class RemoveSublocationsModelValidator : AbstractValidator<RemoveSublocationsModel>
    {
        public RemoveSublocationsModelValidator()
        {
            RuleFor(x => x.ConfirmRemove)
                .NotNull()
                .WithMessage(x => GetNoSelectionMadeErrorMessage(x.Pluralisation))
                .OverridePropertyName("ConfirmRemove");
        }

        public static string GetNoSelectionMadeErrorMessage(string pluralisation)
        {
            return $"Select whether to remove or keep your selected {pluralisation}";
        }
    }
}
