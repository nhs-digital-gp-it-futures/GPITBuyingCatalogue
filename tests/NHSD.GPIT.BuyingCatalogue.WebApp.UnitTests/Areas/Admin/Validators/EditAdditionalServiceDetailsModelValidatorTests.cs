using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditAdditionalServiceDetailsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValidModel_NoValidationErrors(
            EditAdditionalServiceDetailsModel model,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NameNotEntered_HasError(
            Solution solution,
            AdditionalService additionalService,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            additionalService.CatalogueItem.Name = null;

            var model = new EditAdditionalServiceDetailsModel(solution.CatalogueItem, additionalService.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Enter an Additional Service name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_DescriptionNotEntered_HasError(
            Solution solution,
            AdditionalService additionalService,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            additionalService.FullDescription = null;

            var model = new EditAdditionalServiceDetailsModel(solution.CatalogueItem, additionalService.CatalogueItem);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage("Enter an Additional Service description");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ExistingServiceName_HasError(
            Solution solution,
            [Frozen] IAdditionalServicesService additionalServicesService,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            var additionalService = solution.AdditionalServices.First();
            var additionalServiceCatalogueItem = additionalService.CatalogueItem;

            additionalServicesService.AdditionalServiceExistsWithNameForSolution(Arg.Any<string>(), Arg.Any<CatalogueItemId>(), Arg.Any<CatalogueItemId>())
                    .Returns(true);

            var model = new EditAdditionalServiceDetailsModel(solution.CatalogueItem, additionalServiceCatalogueItem)
            {
                Name = solution.AdditionalServices.Last().CatalogueItem.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Additional Service name already exists. Enter a different name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_DuplicatingSolutionName_HasError(
            Solution solution,
            EditAdditionalServiceDetailsModelValidator validator)
        {
            var additionalService = solution.AdditionalServices.First();
            var additionalServiceCatalogueItem = additionalService.CatalogueItem;

            var model = new EditAdditionalServiceDetailsModel(solution.CatalogueItem, additionalServiceCatalogueItem)
            {
                Name = solution.CatalogueItem.Name,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage("Additional Service name cannot be the same as its Catalogue Solution");
        }
    }
}
