using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.InteroperabilityValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddEditGpConnectIntegrationModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValidModel_NoModelError(AddEditGpConnectIntegrationValidator validator)
        {
            var model = new AddEditGpConnectIntegrationModel
            {
                AdditionalInformation = "Additional Information text",
                IsConsumer = false,
                SelectedIntegrationType = 0,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoIntegrationType_SetsModelError(AddEditGpConnectIntegrationValidator validator)
        {
            var model = new AddEditGpConnectIntegrationModel
            {
                AdditionalInformation = "Additional Information text",
                IsConsumer = false,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedIntegrationType)
                .WithErrorMessage("Select integration type");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoProviderOrConsumer_SetsModelError(AddEditGpConnectIntegrationValidator validator)
        {
            var model = new AddEditGpConnectIntegrationModel
            {
                AdditionalInformation = "Additional Information text",
                SelectedIntegrationType = 1,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsConsumer)
                .WithErrorMessage("Select if your system is a provider or consumer");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoAdditionalInformation_SetsModelError(AddEditGpConnectIntegrationValidator validator)
        {
            var model = new AddEditGpConnectIntegrationModel
            {
                SelectedIntegrationType = 2,
                IsConsumer = false,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AdditionalInformation)
                .WithErrorMessage("Enter additional information");
        }
    }
}
