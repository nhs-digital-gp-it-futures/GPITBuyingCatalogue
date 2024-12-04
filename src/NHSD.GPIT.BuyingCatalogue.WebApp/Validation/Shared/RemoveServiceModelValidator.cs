using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared
{
    public class RemoveServiceModelValidator : AbstractValidator<RemoveServiceModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select yes if you want to remove {0}";

        public RemoveServiceModelValidator()
        {
            RuleFor(x => x.ConfirmRemoveService)
                .Must(x => x.HasValue)
                .WithMessage(x => string.Format(NoSelectionMadeErrorMessage, x.ServiceName));
        }
    }
}
