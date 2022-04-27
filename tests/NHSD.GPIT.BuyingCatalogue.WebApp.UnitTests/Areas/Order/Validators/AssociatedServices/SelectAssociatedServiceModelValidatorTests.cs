using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AssociatedServices
{
    public static class SelectAssociatedServiceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedSolutionNull_SetsModelError(
            SelectAssociatedServiceModel model,
            SelectAssociatedServiceModelValidator validator)
        {
            model.SelectedSolutionId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedSolutionId)
                .WithErrorMessage("Select an Associated Service");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            SelectAssociatedServiceModel model,
            SelectAssociatedServiceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
