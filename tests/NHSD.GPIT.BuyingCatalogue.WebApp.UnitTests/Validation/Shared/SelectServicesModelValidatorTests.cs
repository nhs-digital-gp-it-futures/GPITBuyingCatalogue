using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class SelectServicesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NotApplicable_NoValidationErrors(
            SelectServicesModel model,
            SelectServicesModelValidator systemUnderTest)
        {
            model.AssociatedServicesOnly = false;
            model.Services.ForEach(x => x.IsSelected = false);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectServicesModel model,
            SelectServicesModelValidator systemUnderTest)
        {
            model.AssociatedServicesOnly = true;
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectServicesModel model,
            SelectServicesModelValidator systemUnderTest)
        {
            model.AssociatedServicesOnly = true;
            model.Services.ForEach(x => x.IsSelected = false);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Services[0].IsSelected")
                .WithErrorMessage(SelectServicesModelValidator.NoSelectionMadeErrorMessage);
        }
    }
}
