using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Quantity
{
    public static class ServiceRecipientQuantityModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_PatientProvisioning_NonNumericValue_SetsErrorMessage(
            CatalogueItemType catalogueItemType,
            ServiceRecipientQuantityModel model)
        {
            model.InputQuantity = "abc";
            var validator = new ServiceRecipientQuantityModelValidator(ProvisioningType.Patient, catalogueItemType);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(
                    string.Format(
                        ServiceRecipientQuantityModelValidator.PracticeValueNotNumericErrorMessage,
                        model.Name));
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Declarative)]
        [MockInlineAutoData(ProvisioningType.OnDemand)]
        public static void Validate_NonPatientProvisioning_NonNumericValue_SetsErrorMessage(
            ProvisioningType provisioningType,
            CatalogueItemType catalogueItemType,
            ServiceRecipientQuantityModel model)
        {
            model.InputQuantity = "abc";
            var validator = new ServiceRecipientQuantityModelValidator(provisioningType, catalogueItemType);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNotNumericErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(-1)]
        [MockInlineAutoData(0)]
        public static void Validate_SolutionPerPatient_InvalidNumber_SetsErrorMessage(
            int quantity,
            ServiceRecipientQuantityModel model)
        {
            model.InputQuantity = quantity.ToString();
            var validator = new ServiceRecipientQuantityModelValidator(ProvisioningType.Patient, CatalogueItemType.Solution);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(
                    string.Format(
                        ServiceRecipientQuantityModelValidator.PracticeValueGreaterThanZeroErrorMessage,
                        model.Name));
        }

        [Theory]
        [MockInlineAutoData(-1, ProvisioningType.Declarative)]
        [MockInlineAutoData(0, ProvisioningType.Declarative)]
        [MockInlineAutoData(-1, ProvisioningType.OnDemand)]
        [MockInlineAutoData(0, ProvisioningType.OnDemand)]
        public static void Validate_SolutionNonPatientProvisioning_InvalidNumber_SetsErrorMessage(
            int quantity,
            ProvisioningType provisioningType,
            ServiceRecipientQuantityModel model)
        {
            model.InputQuantity = quantity.ToString();
            var validator = new ServiceRecipientQuantityModelValidator(provisioningType, CatalogueItemType.Solution);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueGreaterThanZeroErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(-1)]
        public static void Validate_NonSolutionPerPatient_InvalidNumber_SetsErrorMessage(
            int quantity,
            ServiceRecipientQuantityModel model)
        {
            model.InputQuantity = quantity.ToString();
            var validator = new ServiceRecipientQuantityModelValidator(ProvisioningType.Patient, CatalogueItemType.Solution);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(
                    string.Format(
                        ServiceRecipientQuantityModelValidator.PracticeValueGreaterThanZeroErrorMessage,
                        model.Name));
        }

        [Theory]
        [MockInlineAutoData(-1, CatalogueItemType.AdditionalService, ProvisioningType.Declarative)]
        [MockInlineAutoData(-1, CatalogueItemType.AdditionalService, ProvisioningType.OnDemand)]
        [MockInlineAutoData(-1, CatalogueItemType.AssociatedService, ProvisioningType.Declarative)]
        [MockInlineAutoData(-1, CatalogueItemType.AssociatedService, ProvisioningType.OnDemand)]
        public static void Validate_NonSolutionNonPatientProvisioning_InvalidNumber_SetsErrorMessage(
            int quantity,
            CatalogueItemType catalogueItemType,
            ProvisioningType provisioningType,
            ServiceRecipientQuantityModel model)
        {
            model.InputQuantity = quantity.ToString();
            var validator = new ServiceRecipientQuantityModelValidator(provisioningType, catalogueItemType);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNegativeErrorMessage);
        }
    }
}
