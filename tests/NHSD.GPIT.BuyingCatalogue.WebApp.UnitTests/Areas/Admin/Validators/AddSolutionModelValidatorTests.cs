using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddSolutionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_FrameworkNotValid_SetsModelErrorForListFrameworkModel(
            AddSolutionModelValidator validator,
            AddSolutionModel model)
        {
            model.Frameworks = new List<FrameworkModel> { new() };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor($"{nameof(AddSolutionModel.Frameworks)}[0].Selected")
                .WithErrorMessage("Select the framework(s) your solution is available from");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_FrameworkValid_NoErrorForFrameworkModel(
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            model.Frameworks = new List<FrameworkModel> { new() { Selected = true } };

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Frameworks);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_EmptySupplierId_SetsModelErrorForSupplierId(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            model.SupplierId = null;

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SupplierId)
                .WithErrorMessage("Select a supplier name");

            mockSolutionsService.Verify(s => s.SupplierHasSolutionName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidSupplierId_NoModelErrorForSupplierId(
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task ValidateAsync_SolutionNameValidation_ChecksSolutionNameUnique(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            mockSolutionsService
                .Setup(s => s.SupplierHasSolutionName(model.SupplierId.Value, model.SolutionName))
                .ReturnsAsync(false);

            await validator.TestValidateAsync(model);

            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(model.SupplierId.Value, model.SolutionName));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_SolutionNameNotValid_SetsModelErrorWithoutServiceCall(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            model.SolutionName = string.Empty;

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SolutionName)
                .WithErrorMessage("Enter a solution name");

            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_SolutionNameTooLong_SetsModelErrorWithoutServiceCall(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            model.SolutionName = new string('Z', 256);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SolutionName)
                .WithErrorMessage("Solution name cannot be more than 255 characters");

            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_SolutionNameNotUniqueForSupplierId_SetsModelError(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            mockSolutionsService
                .Setup(s => s.SupplierHasSolutionName(model.SupplierId.Value, model.SolutionName))
                .ReturnsAsync(true);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SolutionName)
                .WithErrorMessage("Solution name already exists. Enter a different solution name");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_SolutionNameUniqueForSupplierId_NoErrorForSupplierName(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            AddSolutionModelValidator validator)
        {
            mockSolutionsService
                .Setup(s => s.SupplierHasSolutionName(model.SupplierId.Value, model.SolutionName))
                .ReturnsAsync(false);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SolutionName);
        }
    }
}
