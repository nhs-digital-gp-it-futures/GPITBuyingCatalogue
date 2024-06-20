using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
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
                SelectedProviderOrConsumer = "Provider",
                SelectedIntegrationType = "Bulk",
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
                SelectedProviderOrConsumer = "Provider",
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
                SelectedIntegrationType = "Bulk",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedProviderOrConsumer)
                .WithErrorMessage("Select if your system is a provider or consumer");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoAdditionalInformation_SetsModelError(AddEditGpConnectIntegrationValidator validator)
        {
            var model = new AddEditGpConnectIntegrationModel
            {
                SelectedIntegrationType = "Bulk",
                SelectedProviderOrConsumer = "Provider",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AdditionalInformation)
                .WithErrorMessage("Enter additional information");
        }
    }
}
