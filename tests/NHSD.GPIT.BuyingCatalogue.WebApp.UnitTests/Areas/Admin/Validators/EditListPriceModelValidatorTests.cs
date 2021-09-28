using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditListPriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidModel_NoValidationErrors(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21M,
                Unit = "per patient",
                UnitDefinition = new string('x', 20),
                SelectedProvisioningType = ProvisioningType.Patient,
            };

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_PriceNotEntered_SetsModelErrorForPrice(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = null,
                Unit = "per patient",
                SelectedProvisioningType = ProvisioningType.Patient,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage("Enter a price");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_NegativePriceValue_SetsModelErrorForPrice(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = -3.21M,
                Unit = "per patient",
                SelectedProvisioningType = ProvisioningType.Patient,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage("Price cannot be negative");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_PriceExceedsDecimalPlaces_SetsModelErrorForPrice(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21145M,
                Unit = "per patient",
                SelectedProvisioningType = ProvisioningType.Patient,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage("Price must be to a maximum of 4 decimal places");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_UnitNotValid_SetsModelErrorForUnit(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21M,
                Unit = null,
                SelectedProvisioningType = ProvisioningType.Patient,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.Unit)
                .WithErrorMessage("Enter a unit");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_UnitDefinitionExceedsCharacterCount_SetsModelErrorForUnitDefinition(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21M,
                Unit = "per patient",
                UnitDefinition = new string('x', 1001),
                SelectedProvisioningType = ProvisioningType.Patient,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.UnitDefinition)
                .WithErrorMessage("Unit definition must be to a maximum of 1,000 characters");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ProvisioningTypeNull_SetsModelErrorForProvisioningType(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21M,
                Unit = "per patient",
                SelectedProvisioningType = null,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.SelectedProvisioningType)
                .WithErrorMessage("Select a provisioning type");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ProvisioningTypeDeclarativeWithNoTimeUnit_SetsModelErrorForProvisioningType(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21M,
                Unit = "per patient",
                SelectedProvisioningType = ProvisioningType.Declarative,
                DeclarativeTimeUnit = null,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.DeclarativeTimeUnit)
                .WithErrorMessage(EditListPriceModelValidator.TimeUnitErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ProvisioningTypeOnDemandWithNoTimeUnit_SetsModelErrorForProvisioningType(
            CatalogueItem solution,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(solution)
            {
                Price = 3.21M,
                Unit = "per patient",
                SelectedProvisioningType = ProvisioningType.OnDemand,
                OnDemandTimeUnit = null,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m.OnDemandTimeUnit)
                .WithErrorMessage(EditListPriceModelValidator.TimeUnitErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_DuplicatePrice_SetsModelError(
            CatalogueItem solution,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            EditListPriceModelValidator validator)
        {
            var cataloguePrice = solution.CataloguePrices.First();
            cataloguePrice.ProvisioningType = ProvisioningType.Patient;
            cataloguePrice.TimeUnit = TimeUnit.PerYear;

            var model = new EditListPriceModel(solution)
            {
                Price = cataloguePrice.Price,
                Unit = cataloguePrice.PricingUnit.Description,
                SelectedProvisioningType = cataloguePrice.ProvisioningType,
            };

            mockSolutionsService
                .Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage("A list price with these details already exists for this Catalogue Solution");
        }
    }
}
