using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AdditionalServices
{
    public static class SelectFlatOnDemandQuantityModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NullEstimationPeriod_ThrowsValidationError(
            SelectFlatOnDemandQuantityModel model,
            SelectFlatOnDemandQuantityModelValidator validator)
        {
            model.EstimationPeriod = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EstimationPeriod)
                .WithErrorMessage("Time Unit is Required");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NullQuantity_ThrowsValidationError(
            SelectFlatOnDemandQuantityModel model,
            SelectFlatOnDemandQuantityModelValidator validator)
        {
            model.EstimationPeriod = TimeUnit.PerMonth;
            model.Quantity = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Enter a quantity");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AlphanumericalQuantity_ThrowsValidationError(
            SelectFlatOnDemandQuantityModel model,
            SelectFlatOnDemandQuantityModelValidator validator)
        {
            model.EstimationPeriod = TimeUnit.PerMonth;
            model.Quantity = "abc";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Quantity must be a number");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_QuantityLessThan1_ThrowsValidationError(
            SelectFlatOnDemandQuantityModel model,
            SelectFlatOnDemandQuantityModelValidator validator)
        {
            model.EstimationPeriod = TimeUnit.PerMonth;
            model.Quantity = "0";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Quantity must be greater than zero");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoValidationErrors(
            SelectFlatOnDemandQuantityModel model,
            SelectFlatOnDemandQuantityModelValidator validator)
        {
            model.EstimationPeriod = TimeUnit.PerMonth;
            model.Quantity = "2";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
