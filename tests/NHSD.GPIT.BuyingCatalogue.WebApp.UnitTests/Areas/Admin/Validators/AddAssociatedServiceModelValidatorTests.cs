using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddAssociatedServiceModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.Name = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(AddAssociatedServiceModelValidator.EnterAssociatedServiceName);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage(AddAssociatedServiceModelValidator.EnterAssociatedServiceDescription);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_OrderGuidanceNullOrEmpty_SetsModelError(
            string orderGuidance,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.OrderGuidance = orderGuidance;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderGuidance)
                .WithErrorMessage(AddAssociatedServiceModelValidator.EnterOrderGuidance);
        }

        [Theory]
        [CommonInlineAutoData(null, null, null)]
        public static void Validate_DataMissing_SetsModelError(
            string name,
            string description,
            string orderGuidance,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.Name = name;
            model.Description = description;
            model.OrderGuidance = orderGuidance;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(AddAssociatedServiceModelValidator.EnterAssociatedServiceName);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage(AddAssociatedServiceModelValidator.EnterAssociatedServiceDescription);

            result.ShouldHaveValidationErrorFor(m => m.OrderGuidance)
                .WithErrorMessage(AddAssociatedServiceModelValidator.EnterOrderGuidance);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DuplicateNameForSupplier_SetsModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(AddAssociatedServiceModelValidator.AssociatedServiceNameAlreadyExists);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidName_NoModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
