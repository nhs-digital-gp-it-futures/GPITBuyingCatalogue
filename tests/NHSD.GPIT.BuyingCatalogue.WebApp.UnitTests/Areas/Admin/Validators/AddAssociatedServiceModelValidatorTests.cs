using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddAssociatedServiceModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.Name = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Enter a name");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter a description");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_OrderGuidanceNullOrEmpty_SetsModelError(
            string orderGuidance,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.OrderGuidance = orderGuidance;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderGuidance)
                .WithErrorMessage("Enter order guidance");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_DuplicateNameForSupplier_SetsModelError(
            [Frozen] IAssociatedServicesService associatedServicesService,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            associatedServicesService.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Associated Service name already exists. Enter a different name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidName_NoModelError(
            [Frozen] IAssociatedServicesService associatedServicesService,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            associatedServicesService.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
