using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.DesktopBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.DesktopBased
{
    public static class MemoryAndStorageModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SelectedMemorySizeNullOrEmpty_SetsModelError(
            string selectedMemorySize,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.SelectedMemorySize = selectedMemorySize;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedMemorySize)
                .WithErrorMessage("Select a minimum memory size");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_StorageSpaceNullOrEmpty_SetsModelError(
            string storageSpace,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.StorageSpace = storageSpace;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.StorageSpace)
                .WithErrorMessage("Enter storage space information");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_ProcessingPowerNullOrEmpty_SetsModelError(
            string processingPower,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.ProcessingPower = processingPower;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ProcessingPower)
                .WithErrorMessage("Enter processing power information");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
