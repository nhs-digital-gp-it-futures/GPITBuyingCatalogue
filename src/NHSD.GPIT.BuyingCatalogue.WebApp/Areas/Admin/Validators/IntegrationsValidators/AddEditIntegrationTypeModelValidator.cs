using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.IntegrationsValidators;

public class AddEditIntegrationTypeModelValidator : AbstractValidator<AddEditIntegrationTypeModel>
{
    internal const string MissingIntegrationTypeNameError = "Enter an integration type name";
    internal const string MissingDescriptionError = "Enter a description";

    internal const string ExistingIntegrationTypeError =
        "An integration type with these details already exists. Enter a different name";

    public AddEditIntegrationTypeModelValidator(
        IIntegrationsService integrationsService)
    {
        ArgumentNullException.ThrowIfNull(integrationsService);

        RuleFor(x => x.IntegrationTypeName)
            .NotEmpty()
            .WithMessage(MissingIntegrationTypeNameError);

        RuleFor(x => x.Description)
            .NotEmpty()
            .When(x => x.RequiresDescription)
            .WithMessage(MissingDescriptionError);

        RuleFor(x => x)
            .Must(
                x => !integrationsService.IntegrationTypeExists(
                        x.IntegrationId.GetValueOrDefault(),
                        x.IntegrationTypeName,
                        x.IntegrationTypeId)
                    .GetAwaiter()
                    .GetResult())
            .When(x => !string.IsNullOrWhiteSpace(x.IntegrationTypeName))
            .WithMessage(ExistingIntegrationTypeError)
            .OverridePropertyName(x => x.IntegrationTypeName);
    }
}
