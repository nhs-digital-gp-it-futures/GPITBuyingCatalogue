using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.SupplierDefinedEpics
{
    public static class SupplierDefinedEpicBaseModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectedCapability_SetsModelError(
            AddSupplierDefinedEpicDetailsModel model,
            AddSupplierDefinedEpicDetailsValidator validator)
        {
            model.SelectedCapabilityIds = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedCapabilityIds)
                .WithErrorMessage("Select a Capability");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            AddSupplierDefinedEpicDetailsModel model,
            AddSupplierDefinedEpicDetailsValidator validator)
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
            AddSupplierDefinedEpicDetailsModel model,
            AddSupplierDefinedEpicDetailsValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter a description");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_IsActiveNull_SetsModelError(
            AddSupplierDefinedEpicDetailsModel model,
            AddSupplierDefinedEpicDetailsValidator validator)
        {
            model.IsActive = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsActive)
                .WithErrorMessage("Select a status");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddingDuplicate_SetsModelError(
            AddSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService service,
            AddSupplierDefinedEpicDetailsValidator validator)
        {
            service.EpicWithNameExists(
                    model.Id,
                    model.Name)
                .Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("An Epic with this name already exists. Try another name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddingNew_NoModelError(
            AddSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService service,
            AddSupplierDefinedEpicDetailsValidator validator)
        {
            service.EpicExists(
                    model.Id,
                    model.Name,
                    model.Description,
                    model.IsActive!.Value)
                .Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
