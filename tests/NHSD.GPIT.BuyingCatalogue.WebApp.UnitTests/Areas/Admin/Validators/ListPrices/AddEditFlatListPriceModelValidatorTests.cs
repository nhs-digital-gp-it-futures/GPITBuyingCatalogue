using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class AddEditFlatListPriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidModel_SetsModelError(
            AddEditFlatListPriceModel model,
            AddEditFlatListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = null;
            model.Price = null;
            model.UnitDescription = null;
            model.SelectedPublicationStatus = null;
            model.SelectedCalculationType = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedProvisioningType)
                .WithErrorMessage(SharedListPriceValidationErrors.SelectedProvisioningTypeError);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceEmptyError);

            result.ShouldHaveValidationErrorFor(m => m.UnitDescription)
                .WithErrorMessage(SharedListPriceValidationErrors.UnitError);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(AddEditFlatListPriceModelValidator.SelectedPublicationStatusError);

            result.ShouldHaveValidationErrorFor(m => m.SelectedCalculationType)
                .WithErrorMessage(SharedListPriceValidationErrors.SelectedCalculationTypeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NegativePrice_SetsModelError(
            AddEditFlatListPriceModel model,
            AddEditFlatListPriceModelValidator validator)
        {
            model.Price = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceNegativeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceGreaterThan4DecimalPlaces_SetsModelError(
                AddEditFlatListPriceModel model,
                AddEditFlatListPriceModelValidator validator)
        {
            model.Price = 1.23456M;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceGreaterThanDecimalPlacesError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_SetsModelError(
            AddEditFlatListPriceModel model,
            [Frozen] Mock<IListPriceService> service,
            AddEditFlatListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = EntityFramework.Catalogue.Models.ProvisioningType.Declarative;

            service.Setup(s => s.HasDuplicateFlatPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.Price!.Value,
                model.UnitDescription)).ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor($"{nameof(model.SelectedProvisioningType)}|{nameof(model.Price)}|{nameof(model.UnitDescription)}|{nameof(model.SelectedCalculationType)}")
                .WithErrorMessage(SharedListPriceValidationErrors.DuplicateListPriceError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            AddEditFlatListPriceModel model,
            [Frozen] Mock<IListPriceService> service,
            AddEditFlatListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = EntityFramework.Catalogue.Models.ProvisioningType.Declarative;

            service.Setup(s => s.HasDuplicateFlatPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.Price!.Value,
                model.UnitDescription)).ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
