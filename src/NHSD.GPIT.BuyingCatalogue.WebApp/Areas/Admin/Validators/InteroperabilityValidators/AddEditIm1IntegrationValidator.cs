using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.InteroperabilityValidators
{
    public sealed class AddEditIm1IntegrationValidator : AbstractValidator<AddEditIm1IntegrationModel>
    {
        public const string SelectIntegrationTypeError = "Select integration type";
        public const string SelectProviderOrConsumerError = "Select if your system is a provider or consumer";
        public const string IntegratesWithError = "Enter the system being integrated with";
        public const string DescriptionError = "Enter a description";

        public AddEditIm1IntegrationValidator()
        {
            RuleFor(i => i.SelectedIntegrationType)
                .NotNull()
                .WithMessage(SelectIntegrationTypeError);

            RuleFor(i => i.SelectedProviderOrConsumer)
                .NotNull()
                .WithMessage(SelectProviderOrConsumerError);

            RuleFor(i => i.IntegratesWith)
                .NotEmpty()
                .WithMessage(IntegratesWithError);

            RuleFor(i => i.Description)
                .NotEmpty()
                .WithMessage(DescriptionError);
        }
    }
}
