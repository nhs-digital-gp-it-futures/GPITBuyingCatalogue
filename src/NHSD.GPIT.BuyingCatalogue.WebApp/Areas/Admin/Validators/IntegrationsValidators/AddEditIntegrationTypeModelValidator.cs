using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.IntegrationsValidators;

public class AddEditIntegrationTypeModelValidator : AbstractValidator<AddEditIntegrationTypeModel>
{
    public AddEditIntegrationTypeModelValidator(
        IIntegrationsService integrationsService)
    {
        ArgumentNullException.ThrowIfNull(integrationsService);

        RuleFor(x => x.IntegrationTypeName)
            .NotEmpty()
            .WithMessage("Enter an integration type name");

        RuleFor(x => x.Description)
            .NotEmpty()
            .When(x => x.RequiresDescription)
            .WithMessage("Enter a description");

        RuleFor(x => x)
            .Must(
                x => !integrationsService.IntegrationTypeExists(
                        x.IntegrationId.GetValueOrDefault(),
                        x.IntegrationTypeName,
                        x.IntegrationTypeId)
                    .GetAwaiter()
                    .GetResult())
            .When(x => !string.IsNullOrWhiteSpace(x.IntegrationTypeName))
            .WithMessage("An integration type with these details already exists. Enter a different name")
            .OverridePropertyName(x => x.IntegrationTypeName);
    }
}
