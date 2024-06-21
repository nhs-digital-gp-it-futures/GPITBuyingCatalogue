using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.InteroperabilityValidators
{
    public class AddEditGpConnectIntegrationValidator : AbstractValidator<AddEditGpConnectIntegrationModel>
    {
        public AddEditGpConnectIntegrationValidator()
        {
            RuleFor(i => i.SelectedIntegrationType)
                .NotNull()
                .WithMessage("Select integration type");

            RuleFor(i => i.IsConsumer)
                .NotNull()
                .WithMessage("Select if your system is a provider or consumer");

            RuleFor(i => i.AdditionalInformation)
                .NotEmpty()
                .WithMessage("Enter additional information");
        }
    }
}
