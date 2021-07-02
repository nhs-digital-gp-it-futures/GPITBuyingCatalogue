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
        public static void Validate_FrameworkNotValid_SetsModelErrorForFrameworkModel(
            AddSolutionModelValidator validator,
            [Frozen] Mock<FrameworkModel> mockFrameworkModel,
            AddSolutionModel model)
        {
            mockFrameworkModel.Setup(f => f.IsValid())
                .Returns(false);
            model.FrameworkModel = mockFrameworkModel.Object;
            
            validator.ShouldHaveValidationErrorFor(m => m.FrameworkModel, model)
                .WithErrorMessage("Select the framework(s) your solution is available from");
            mockFrameworkModel.Verify(f => f.IsValid());
        }
        
        [Theory]
        [CommonAutoData]
        public static void Validate_FrameworkValid_NoErrorForFrameworkModel(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<FrameworkModel> mockFrameworkModel,
            AddSolutionModel model)
        {
            mockFrameworkModel.Setup(f => f.IsValid())
                .Returns(true);
            model.FrameworkModel = mockFrameworkModel.Object;

            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldNotHaveValidationErrorFor(m => m.FrameworkModel, model);
            mockFrameworkModel.Verify(f => f.IsValid());
        }
        
        [Theory]
        [CommonAutoData]
        public static async Task Validate_EmptySupplierId_SetsModelErrorForSupplierId(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        {
            model.SupplierId = string.Empty;

            await new AddSolutionModelValidator(mockSolutionsService.Object)
                .ValidateAsync(model);
            
            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldHaveValidationErrorFor(m => m.SupplierId, model)
                .WithErrorMessage("Select a supplier name");
            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_InvalidSupplierId_SetsModelErrorForSupplierId(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model,
            string invalidSupplierId)
        {
            model.SupplierId = invalidSupplierId;

            await new AddSolutionModelValidator(mockSolutionsService.Object)
                .ValidateAsync(model);
            
            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldHaveValidationErrorFor(m => m.SupplierId, model)
                .WithErrorMessage("Supplier Id should be a valid integer");
            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        
        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidSupplierId_NoModelErrorForSupplierId(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        {
            await new AddSolutionModelValidator(mockSolutionsService.Object)
                .ValidateAsync(model);
            
            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldNotHaveValidationErrorFor(m => m.SupplierId, model);
        }
        
        [Theory]
        [CommonAutoData]
        public static async Task ValidateAsync_SolutionNameValidation_ChecksSolutionNameUnique(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        {
            mockSolutionsService
                .Setup(s => s.SupplierHasSolutionName(model.SupplierId, model.SolutionName))
                .ReturnsAsync(false);
            
            await new AddSolutionModelValidator(mockSolutionsService.Object)
                .ValidateAsync(model);
            
            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(model.SupplierId, model.SolutionName));
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SolutionNameNotValid_SetsModelErrorWithoutServiceCall(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        {
            model.SolutionName = string.Empty;

            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldHaveValidationErrorFor(m => m.SolutionName, model)
                .WithErrorMessage("Enter a solution name");
            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        
        [Theory]
        [CommonAutoData]
        public static void Validate_SolutionNameTooLong_SetsModelErrorWithoutServiceCall(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        {
            model.SolutionName = new string('Z', 256);

            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldHaveValidationErrorFor(m => m.SolutionName, model)
                .WithErrorMessage("Solution name cannot be more than 255 characters");
            mockSolutionsService
                .Verify(s => s.SupplierHasSolutionName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        
        [Theory]
        [CommonAutoData]
        public static void Validate_SolutionNameNotUniqueForSupplierId_SetsModelError(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        { 
            mockSolutionsService
                .Setup(s => s.SupplierHasSolutionName(model.SupplierId, model.SolutionName))
                .ReturnsAsync(true);

            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldHaveValidationErrorFor(m => m.SolutionName, model)
                .WithErrorMessage("Solution name already exists. Enter a different solution name");
        }
        
        [Theory]
        [CommonAutoData]
        public static void Validate_SolutionNameUniqueForSupplierId_NoErrorForSupplierName(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AddSolutionModel model)
        {
            mockSolutionsService
                .Setup(s => s.SupplierHasSolutionName(model.SupplierId, model.SolutionName))
                .ReturnsAsync(false);
            
            new AddSolutionModelValidator(mockSolutionsService.Object)
                .ShouldNotHaveValidationErrorFor(m => m.SolutionName, model);
        }
    }
}
