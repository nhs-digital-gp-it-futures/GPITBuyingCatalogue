using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.InteroperabilityValidators
{
    public sealed class AddEditIm1IntegrationValidator : AbstractValidator<AddEditIm1IntegrationModel>
    {
        public AddEditIm1IntegrationValidator()
        {
            RuleFor(i => i.SelectedIntegrationType)
                .NotNull()
                .WithMessage("Select integration type");

            RuleFor(i => i.IsConsumer)
                .NotNull()
                .WithMessage("Select if your system is a provider or consumer");

            RuleFor(i => i.IntegratesWith)
                .NotEmpty()
                .WithMessage("Enter the system being integrated with");

            RuleFor(i => i.Description)
                .NotEmpty()
                .WithMessage("Enter a description");
        }
    }
}
