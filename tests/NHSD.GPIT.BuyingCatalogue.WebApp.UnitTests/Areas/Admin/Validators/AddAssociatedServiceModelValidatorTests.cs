using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Microsoft.CodeAnalysis;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class AddAssociatedServiceModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
        [CommonAutoData]
        public static void Validate_DuplicateNameForSupplier_SetsModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Associated Service name already exists. Enter a different name");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidName_NoModelError(
            [Frozen] Mock<IAssociatedServicesService> associatedServicesService,
            AddAssociatedServiceModel model,
            AddAssociatedServiceModelValidator validator)
        {
            model.IdDisplay = model.SolutionId.SupplierId + "-S-123";

            associatedServicesService.Setup(s => s.GetAssociatedService(model.Id.GetValueOrDefault()))
                .ReturnsAsync((CatalogueItem)null);

            associatedServicesService.Setup(s => s.AssociatedServiceExistsWithNameForSupplier(model.Name, model.SolutionId.SupplierId, default))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
