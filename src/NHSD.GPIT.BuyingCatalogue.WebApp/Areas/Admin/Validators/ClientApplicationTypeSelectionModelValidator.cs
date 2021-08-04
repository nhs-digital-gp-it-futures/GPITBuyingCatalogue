using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class ClientApplicationTypeSelectionModelValidator : AbstractValidator<ClientApplicationTypeSelectionModel>
    {
        internal const string SelectionErrorMessage = "Select an application type";

        public ClientApplicationTypeSelectionModelValidator()
        {
            RuleFor(m => m.SelectedApplicationType)
                .NotNull()
                .WithMessage(SelectionErrorMessage)
                .When(m => m.ApplicationTypesAvailableForSelection);
        }
    }
}
