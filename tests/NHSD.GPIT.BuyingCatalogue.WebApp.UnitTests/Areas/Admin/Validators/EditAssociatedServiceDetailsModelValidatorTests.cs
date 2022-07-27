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
    public static class EditAssociatedServiceDetailsModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.Name = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.NameError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.DescriptionError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_OrderGuidanceNullOrEmpty_SetsModelError(
            string orderGuidance,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.OrderGuidance = orderGuidance;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderGuidance)
                .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.OrderGuidanceError);
        }

        [Theory]
        [CommonInlineAutoData(null, null, null)]
        [CommonInlineAutoData("", "", "")]
        public static void Validate_AllFieldsNullOrEmpty_SetsModelError(
            string name,
            string description,
            string orderGuidance,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            model.Name = name;
            model.Description = description;
            model.OrderGuidance = orderGuidance;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
               .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.NameError);

            result.ShouldHaveValidationErrorFor(m => m.Description)
               .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.DescriptionError);

            result.ShouldHaveValidationErrorFor(m => m.OrderGuidance)
                .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.OrderGuidanceError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DuplicateNameForSupplier_SetsModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, model.Id.Value))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(EditAssociatedServiceDetailsModelValidator.NameAlreadyExistsError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidName_NoModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            EditAssociatedServiceDetailsModel model,
            EditAssociatedServiceDetailsModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
