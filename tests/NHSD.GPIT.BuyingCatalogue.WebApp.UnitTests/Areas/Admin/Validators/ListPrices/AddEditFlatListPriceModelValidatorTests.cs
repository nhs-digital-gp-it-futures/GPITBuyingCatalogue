using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class AddEditFlatListPriceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_InvalidModel_SetsModelError(
            AddEditFlatListPriceModel model,
            AddEditFlatListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = null;
            model.InputPrice = null;
            model.UnitDescription = null;
            model.SelectedPublicationStatus = null;
            model.SelectedCalculationType = null;
            model.RangeDefinition = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedProvisioningType)
                .WithErrorMessage(SharedListPriceValidationErrors.SelectedProvisioningTypeError);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceEmptyError);

            result.ShouldHaveValidationErrorFor(m => m.UnitDescription)
                .WithErrorMessage(SharedListPriceValidationErrors.UnitError);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPublicationStatus)
                .WithErrorMessage(AddEditFlatListPriceModelValidator.SelectedPublicationStatusError);

            result.ShouldHaveValidationErrorFor(m => m.SelectedCalculationType)
                .WithErrorMessage(SharedListPriceValidationErrors.SelectedCalculationTypeError);

            result.ShouldHaveValidationErrorFor(m => m.RangeDefinition)
                .WithErrorMessage(SharedListPriceValidationErrors.UnitsError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NegativePrice_SetsModelError(
            AddEditFlatListPriceModel model,
            AddEditFlatListPriceModelValidator validator)
        {
            model.InputPrice = "-1";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceNegativeError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PriceGreaterThan4DecimalPlaces_SetsModelError(
            AddEditFlatListPriceModel model,
            AddEditFlatListPriceModelValidator validator)
        {
            model.InputPrice = "1.23456";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceGreaterThanDecimalPlacesError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PriceNotNumeric_SetsModelError(
            AddEditFlatListPriceModel model,
            AddEditFlatListPriceModelValidator validator)
        {
            model.InputPrice = "abc";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceNotANumberError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Duplicate_SetsModelError(
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService service,
            AddEditFlatListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = EntityFramework.Catalogue.Models.ProvisioningType.Declarative;
            model.InputPrice = "3.14";

            service.HasDuplicateFlatPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.Price!.Value,
                model.UnitDescription).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor($"{nameof(model.SelectedProvisioningType)}|{nameof(model.InputPrice)}|{nameof(model.UnitDescription)}|{nameof(model.SelectedCalculationType)}")
                .WithErrorMessage(SharedListPriceValidationErrors.DuplicateListPriceError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            AddEditFlatListPriceModel model,
            [Frozen] IListPriceService service,
            AddEditFlatListPriceModelValidator validator)
        {
            model.SelectedProvisioningType = EntityFramework.Catalogue.Models.ProvisioningType.Declarative;
            model.InputPrice = "3.14";

            service.HasDuplicateFlatPrice(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.Price!.Value,
                model.UnitDescription).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
