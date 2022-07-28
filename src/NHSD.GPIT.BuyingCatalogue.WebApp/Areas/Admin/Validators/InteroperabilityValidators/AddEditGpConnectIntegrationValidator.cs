using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.InteroperabilityValidators
{
    public class AddEditGpConnectIntegrationValidator : AbstractValidator<AddEditGpConnectIntegrationModel>
    {
        public const string SelectIntegrationTypeError = "Select integration type";
        public const string SelectProviderOrConsumerError = "Select if your system is a provider or consumer";
        public const string AdditionalInformationError = "Enter additional information";

        public AddEditGpConnectIntegrationValidator()
        {
            RuleFor(i => i.SelectedIntegrationType)
                .NotNull()
                .WithMessage(SelectIntegrationTypeError);

            RuleFor(i => i.SelectedProviderOrConsumer)
                .NotNull()
                .WithMessage(SelectProviderOrConsumerError);

            RuleFor(i => i.AdditionalInformation)
                .NotEmpty()
                .WithMessage(AdditionalInformationError);
        }
    }
}
