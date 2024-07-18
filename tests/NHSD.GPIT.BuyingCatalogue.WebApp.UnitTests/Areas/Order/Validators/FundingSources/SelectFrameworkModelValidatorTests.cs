using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.FundingSources
{
    public static class SelectFrameworkModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_SelectedFrameworkNotSelected_SetsModelError(
            SelectFrameworkModel model,
            SelectFrameworkModelValidator validator)
        {
            model.SelectedFramework = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedFramework)
                .WithErrorMessage(SelectFrameworkModelValidator.SelectFrameworkErrorMessage);
        }
    }
}
