using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAdditionalServiceDetailsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidModel_NoValidationErrors(
            EditAdditionalServiceDetailsModel model,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_NameNotEntered_HasError(
            CatalogueItem catalogueItem,
            CatalogueItem additionalService,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            additionalService.Name = null;

            var model = new EditAdditionalServiceDetailsModel(catalogueItem, additionalService);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Enter an Additional Service name");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_DescriptionNotEntered_HasError(
            CatalogueItem catalogueItem,
            CatalogueItem additionalService,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            additionalService.AdditionalService.FullDescription = null;

            var model = new EditAdditionalServiceDetailsModel(catalogueItem, additionalService);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter an Additional Service description");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ExistingServiceName_HasError(
            CatalogueItem catalogueItem,
            CatalogueItem additionalService,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            suppliersService.Setup(s => s.GetAllSolutionsForSupplier(catalogueItem.Supplier.Id))
                .ReturnsAsync(new List<CatalogueItem> { additionalService });

            var model = new EditAdditionalServiceDetailsModel(catalogueItem, additionalService);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Additional Service name already exists. Enter a different name");
        }
    }
}
