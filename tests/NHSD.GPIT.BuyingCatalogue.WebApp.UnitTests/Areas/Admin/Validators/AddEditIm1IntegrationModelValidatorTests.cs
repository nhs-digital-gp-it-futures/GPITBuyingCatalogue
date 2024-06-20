using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.InteroperabilityValidators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddEditIm1IntegrationModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValidModel_NoModelError(AddEditIm1IntegrationValidator validator)
        {
            var model = new AddEditIm1IntegrationModel
            {
                Description = "Description Text",
                SelectedProviderOrConsumer = "Provider",
                SelectedIntegrationType = "Bulk",
                IntegratesWith = "Integrates With Text",
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoIntegrationType_SetsModelError(AddEditIm1IntegrationValidator validator)
        {
            var model = new AddEditIm1IntegrationModel
            {
                Description = "Description Text",
                IntegratesWith = "Integrates With Text",
                SelectedProviderOrConsumer = "Provider",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedIntegrationType)
                .WithErrorMessage("Select integration type");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoProviderOrConsumer_SetsModelError(AddEditIm1IntegrationValidator validator)
        {
            var model = new AddEditIm1IntegrationModel
            {
                Description = "Description Text",
                IntegratesWith = "Integrates With Text",
                SelectedIntegrationType = "Bulk",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedProviderOrConsumer)
                .WithErrorMessage("Select if your system is a provider or consumer");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoDescription_SetsModelError(AddEditIm1IntegrationValidator validator)
        {
            var model = new AddEditIm1IntegrationModel
            {
                SelectedIntegrationType = "Bulk",
                IntegratesWith = "Integrates With Text",
                SelectedProviderOrConsumer = "Provider",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter a description");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoIntegratesWith_SetsModelError(AddEditIm1IntegrationValidator validator)
        {
            var model = new AddEditIm1IntegrationModel
            {
                Description = "Description Text",
                SelectedProviderOrConsumer = "Provider",
                SelectedIntegrationType = "Bulk",
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IntegratesWith)
                .WithErrorMessage("Enter the system being integrated with");
        }
    }
}
