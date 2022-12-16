using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.FundingSources
{
    public static class ConfirmFrameworkChangeModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_ConfirmChangesNotSelected_SetsModelError(
            ConfirmFrameworkChangeModel model,
            ConfirmFrameworkChangeModelValidator validator)
        {
            model.ConfirmChanges = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ConfirmChanges)
                .WithErrorMessage(ConfirmFrameworkChangeModelValidator.ConfirmChangeErrorMessage);
        }
    }
}
