using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class ApplicationTypeSelectionModelValidator : AbstractValidator<ApplicationTypeSelectionModel>
    {
        internal const string SelectionErrorMessage = "Select an application type";

        public ApplicationTypeSelectionModelValidator()
        {
            RuleFor(m => m.SelectedApplicationType)
                .NotNull()
                .WithMessage(SelectionErrorMessage)
                .When(m => m.ApplicationTypesAvailableForSelection);
        }
    }
}
