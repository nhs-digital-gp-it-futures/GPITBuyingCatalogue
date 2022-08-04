using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class TriageDueDiligenceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectYes_NoModelError(
            TriageDueDiligenceModel model,
            TriageDueDiligenceModelValidator validator)
        {
            model.Selected = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectNo_NoModelError(
            TriageDueDiligenceModel model,
            TriageDueDiligenceModelValidator validator)
        {
            model.Selected = false;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelection_ThrowsError(
            TriageDueDiligenceModel model,
            TriageDueDiligenceModelValidator validator)
        {
            model.Selected = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Selected)
                .WithErrorMessage(TriageDueDiligenceModelValidator.NoSelectionErrorMessage);
        }
    }
}
