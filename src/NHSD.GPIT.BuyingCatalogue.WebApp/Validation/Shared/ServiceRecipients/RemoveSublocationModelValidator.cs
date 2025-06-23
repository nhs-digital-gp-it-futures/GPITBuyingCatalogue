using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients
{
    public class RemoveSublocationModelValidator : AbstractValidator<RemoveSublocationsModel>
    {
        public const string NoSelectionMadeMessage = "Select whether to keep or remove your selection";

        public RemoveSublocationModelValidator()
        {
            RuleFor(x => x.ConfirmRemove)
                .NotNull()
                .WithMessage(NoSelectionMadeMessage)
                .OverridePropertyName("ConfirmRemove_0");
        }
    }
}
