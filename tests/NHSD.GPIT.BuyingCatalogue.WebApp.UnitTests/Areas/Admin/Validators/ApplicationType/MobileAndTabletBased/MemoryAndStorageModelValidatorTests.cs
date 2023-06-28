using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.MobileTabletBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.MobileAndTabletBased
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
                .WithErrorMessage("Select a minimum memory size");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            MemoryAndStorageModel model,
            MemoryAndStorageModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter storage space information");
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
