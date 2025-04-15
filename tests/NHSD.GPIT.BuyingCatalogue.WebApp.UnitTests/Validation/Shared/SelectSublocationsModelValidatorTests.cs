using System.Collections.Generic;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class SelectSublocationsModelValidatorTests
    {
        [Theory]
        [MockMemberAutoData(nameof(NoValidationErrorsSublocation))]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectSublocationsModel model,
            SelectSublocationsModelValidator systemUnderTest)
        {
            TestValidationResult<SelectSublocationsModel> result = systemUnderTest.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockMemberAutoData(nameof(ValidationErrorsSublocation))]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectSublocationsModel model,
            SelectSublocationsModelValidator systemUnderTest)
        {
            TestValidationResult<SelectSublocationsModel> result = systemUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor("RenderedSublocations[0].Value")
                .WithErrorMessage(SelectSublocationsModelValidator.NoSublocationSelectedMessage);
        }

        public static IEnumerable<object[]> NoValidationErrorsSublocation()
        {
            return
            [
                [
                    new SelectSublocationsModel
                    {
                        RenderedSublocations =
                            [new SelectOption<string> { Text = "Option 1", Value = "XXXX", Selected = true }],
                    },
                ],
            ];
        }

        public static IEnumerable<object[]> ValidationErrorsSublocation()
        {
            return
            [
                [
                    new SelectSublocationsModel
                    {
                        RenderedSublocations =
                            [new SelectOption<string> { Text = "Option 1", Value = "XXXX", Selected = false }],
                    },
                ],
            ];
        }
    }
}
