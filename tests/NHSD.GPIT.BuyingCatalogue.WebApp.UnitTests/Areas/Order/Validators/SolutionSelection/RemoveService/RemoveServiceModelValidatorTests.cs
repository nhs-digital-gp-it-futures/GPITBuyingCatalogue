﻿using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.RemoveService;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.RemoveService
{
    public static class RemoveServiceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            RemoveServiceModel model,
            RemoveServiceModelValidator systemUnderTest)
        {
            model.ConfirmRemoveService = null;

            var result = systemUnderTest.TestValidate(model);

            string expectedError = string.Format(RemoveServiceModelValidator.NoSelectionMadeErrorMessage, model.ServiceName);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmRemoveService)
                .WithErrorMessage(expectedError);
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Validate_ValuesProvided_NoValidationErrors(
            bool selection,
            RemoveServiceModel model,
            RemoveServiceModelValidator systemUnderTest)
        {
            model.ConfirmRemoveService = selection;
            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
