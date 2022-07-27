using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.DesktopBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ClientApplicationType.DesktopBased
{
    public static class MemoryAndStorageModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_SelectedMemorySizeNullOrEmpty_SetsModelError(
            string selectedMemorySize,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.SelectedMemorySize = selectedMemorySize;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedMemorySize)
                .WithErrorMessage(MemoryAndStorageModelValidator.SelectMinimumMemorySizeError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_StorageSpaceNullOrEmpty_SetsModelError(
            string storageSpace,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.StorageSpace = storageSpace;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.StorageSpace)
                .WithErrorMessage(MemoryAndStorageModelValidator.StorageSpaceInformationError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_ProcessingPowerNullOrEmpty_SetsModelError(
            string processingPower,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.ProcessingPower = processingPower;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ProcessingPower)
                .WithErrorMessage(MemoryAndStorageModelValidator.ProcessingPowerInformationError);
        }

        [Theory]
        [CommonInlineAutoData(null, null, null)]
        [CommonInlineAutoData("", "", "")]
        public static void Validate_AllDataNullOrEmpty_SetsModelError(
            string selectedMemorySize,
            string storageSpace,
            string processingPower,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.SelectedMemorySize = selectedMemorySize;
            model.StorageSpace = storageSpace;
            model.ProcessingPower = processingPower;

            var result = validator.TestValidate(model);


            result.ShouldHaveValidationErrorFor(m => m.SelectedMemorySize)
                .WithErrorMessage(MemoryAndStorageModelValidator.SelectMinimumMemorySizeError);

            result.ShouldHaveValidationErrorFor(m => m.StorageSpace)
                .WithErrorMessage(MemoryAndStorageModelValidator.StorageSpaceInformationError);


            result.ShouldHaveValidationErrorFor(m => m.ProcessingPower)
                .WithErrorMessage(MemoryAndStorageModelValidator.ProcessingPowerInformationError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
