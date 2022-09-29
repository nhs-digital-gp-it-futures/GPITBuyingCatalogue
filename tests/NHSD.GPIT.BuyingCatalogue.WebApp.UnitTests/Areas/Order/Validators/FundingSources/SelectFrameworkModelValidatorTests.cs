using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.FundingSources
{
    public static class SelectFrameworkModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
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
