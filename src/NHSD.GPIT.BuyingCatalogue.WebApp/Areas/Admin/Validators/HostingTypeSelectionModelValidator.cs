using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class HostingTypeSelectionModelValidator : AbstractValidator<HostingTypeSelectionModel>
    {
        internal const string SelectionErrorMessage = "Select a hosting type";

        public HostingTypeSelectionModelValidator()
        {
            RuleFor(m => m.SelectedHostingType)
                .NotNull()
                .WithMessage(SelectionErrorMessage)
                .When(m => m.HostingTypesAvailableForSelection);
        }
    }
}
