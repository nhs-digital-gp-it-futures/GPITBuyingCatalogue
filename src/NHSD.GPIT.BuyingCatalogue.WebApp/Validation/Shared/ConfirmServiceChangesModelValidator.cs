using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared
{
    public class ConfirmServiceChangesModelValidator : AbstractValidator<ConfirmServiceChangesModel>
    {
        public const string ErrorMessage = "Select yes if you want to confirm your changes";

        public ConfirmServiceChangesModelValidator()
        {
            RuleFor(x => x.ConfirmChanges)
                .NotNull()
                .WithMessage(ErrorMessage);
        }
    }
}
