﻿using System.Collections.Generic;
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
    public static class SolutionModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_FrameworkNotValid_SetsModelErrorForListFrameworkModel(
            SolutionModelValidator validator,
            SolutionModel model)
        {
            model.Frameworks = new List<FrameworkModel> { new() };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithErrorMessage("Select the framework(s) your solution is available from");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_FrameworkValid_NoErrorForFrameworkModel(
            SolutionModel model,
            SolutionModelValidator validator)
        {
            model.Frameworks = new List<FrameworkModel> { new() { Selected = true } };

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Frameworks);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_EmptySupplierId_SetsModelErrorForSupplierId(
            SolutionModel model,
            SolutionModelValidator validator)
        {
            model.SupplierId = null;

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SupplierId)
                .WithErrorMessage("Select a supplier name");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidSupplierId_NoModelErrorForSupplierId(
            SolutionModel model,
            SolutionModelValidator validator)
        {
            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_SolutionNameNotValid_SetsModelError(
            SolutionModel model,
            SolutionModelValidator validator)
        {
            model.SolutionName = string.Empty;

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SolutionName)
                .WithErrorMessage("Enter a solution name");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_SolutionNameTooLong_SetsModelError(
            SolutionModel model,
            SolutionModelValidator validator)
        {
            model.SolutionName = new string('Z', 256);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SolutionName)
                .WithErrorMessage("Solution name cannot be more than 255 characters");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_AddSolutionNameAlreadyExists_SetsModelErrorForSolutionName(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionModel model,
            SolutionModelValidator validator)
        {
            model.SolutionId = null;

            mockSolutionsService.Setup(s => s.CatalogueSolutionExistsWithName(model.SolutionName, default))
                .ReturnsAsync(true);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.SolutionName);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_EditSolutionNameAlreadyExists_SetsModelErrorForSolutionName(
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            SolutionModel model,
            SolutionModelValidator validator)
        {
            mockSolutionsService.Setup(s => s.CatalogueSolutionExistsWithName(model.SolutionName, model.SolutionId.Value))
                .ReturnsAsync(true);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.SolutionName);
        }
    }
}
