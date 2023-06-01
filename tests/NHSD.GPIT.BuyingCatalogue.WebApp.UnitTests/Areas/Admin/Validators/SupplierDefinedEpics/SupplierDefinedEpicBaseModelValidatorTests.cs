using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.SupplierDefinedEpics
{
    public static class SupplierDefinedEpicBaseModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectedCapability_SetsModelError(
            SupplierDefinedEpicBaseModel model,
            SupplierDefinedEpicBaseModelValidator validator)
        {
            model.SelectedCapabilityId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedCapabilityId)
                .WithErrorMessage("Select a Capability");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            SupplierDefinedEpicBaseModel model,
            SupplierDefinedEpicBaseModelValidator validator)
        {
            model.Name = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Enter an Epic name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            SupplierDefinedEpicBaseModel model,
            SupplierDefinedEpicBaseModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter a description");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_IsActiveNull_SetsModelError(
            SupplierDefinedEpicBaseModel model,
            SupplierDefinedEpicBaseModelValidator validator)
        {
            model.IsActive = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage("Select a status");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddingDuplicate_SetsModelError(
            SupplierDefinedEpicBaseModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            SupplierDefinedEpicBaseModelValidator validator)
        {
            service.Setup(s =>
                s.EpicExists(
                    model.Id,
                    model.Name,
                    model.Description,
                    model.IsActive!.Value))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            var expectedErrorProperty = string.Format(
                "{0}|{1}|{2}|{3}",
                nameof(model.SelectedCapabilityId),
                nameof(model.Name),
                nameof(model.Description),
                nameof(model.IsActive));

            result.ShouldHaveValidationErrorFor(expectedErrorProperty)
                .WithErrorMessage("A supplier defined Epic with these details already exists");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddingNew_NoModelError(
            SupplierDefinedEpicBaseModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> service,
            SupplierDefinedEpicBaseModelValidator validator)
        {
            service.Setup(s =>
                s.EpicExists(
                    model.Id,
                    model.Name,
                    model.Description,
                    model.IsActive!.Value))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
