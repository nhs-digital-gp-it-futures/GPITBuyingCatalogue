using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
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
        public static async Task Validate_ProvisioningTypeOnDemandWithNoTimeUnit_NoValidationError(
            CatalogueItem item,
            EditListPriceModelValidator validator)
        {
            var model = new EditListPriceModel(item)
            {
                Price = 3.21M,
                Unit = "per patient",
                SelectedProvisioningType = ProvisioningType.OnDemand,
                OnDemandTimeUnit = null,
            };

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldNotHaveValidationErrorFor(m => m.OnDemandTimeUnit);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_DuplicatePrice_SetsModelError(
            CatalogueItem item,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            EditListPriceModelValidator validator)
        {
            var cataloguePrice = item.CataloguePrices.First();
            cataloguePrice.ProvisioningType = ProvisioningType.Patient;
            cataloguePrice.TimeUnit = TimeUnit.PerYear;

            var model = new EditListPriceModel(item)
            {
                Price = cataloguePrice.Price,
                Unit = cataloguePrice.PricingUnit.Description,
                SelectedProvisioningType = cataloguePrice.ProvisioningType,
            };

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(item.Id))
                .ReturnsAsync(item);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldHaveValidationErrorFor(m => m)
                .WithErrorMessage("A list price with these details already exists for this Catalogue Solution");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_DuplicatePriceIsCurrentPrice_NoValidationError(
            CatalogueItem item,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            EditListPriceModelValidator validator)
        {
            var cataloguePrice = item.CataloguePrices.First();
            cataloguePrice.ProvisioningType = ProvisioningType.Patient;
            cataloguePrice.TimeUnit = TimeUnit.PerYear;

            var model = new EditListPriceModel(item)
            {
                CataloguePriceId = cataloguePrice.CataloguePriceId,
                Price = cataloguePrice.Price,
                Unit = cataloguePrice.PricingUnit.Description,
                SelectedProvisioningType = cataloguePrice.ProvisioningType,
            };

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(item.Id))
                .ReturnsAsync(item);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_DuplicateOnDemandWithNoTimeUnit_NoValidationError(
            CatalogueItem item,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            EditListPriceModelValidator validator)
        {
            var cataloguePrice = item.CataloguePrices.First();
            cataloguePrice.ProvisioningType = ProvisioningType.OnDemand;
            cataloguePrice.TimeUnit = null;

            var model = new EditListPriceModel(item)
            {
                CataloguePriceId = cataloguePrice.CataloguePriceId,
                Price = cataloguePrice.Price,
                Unit = cataloguePrice.PricingUnit.Description,
                SelectedProvisioningType = cataloguePrice.ProvisioningType,
                OnDemandTimeUnit = null,
            };

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(item.Id))
                .ReturnsAsync(item);

            var result = await validator.TestValidateAsync(model);

            result
                .ShouldNotHaveValidationErrorFor(m => m);
        }
    }
}
