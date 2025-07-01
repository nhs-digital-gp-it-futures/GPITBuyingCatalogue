using System.Collections.Generic;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class SelectSublocationRecipientsModelValidatorTests
    {
        [Theory]
        [MockMemberAutoData(nameof(NoValidationErrorsSublocationRecipients))]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectSublocationRecipientsModel model,
            SelectSublocationRecipientsModelValidator systemUnderTest)
        {
            TestValidationResult<SelectSublocationRecipientsModel> result = systemUnderTest.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockMemberAutoData(nameof(ValidationErrorsSublocationRecipients))]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectSublocationRecipientsModel model,
            SelectSublocationRecipientsModelValidator systemUnderTest)
        {
            TestValidationResult<SelectSublocationRecipientsModel> result = systemUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor("RenderedServiceRecipients[0].Selected")
                .WithErrorMessage(SelectSublocationRecipientsModelValidator.NoRecipientsSelectedMessage);
        }

        public static IEnumerable<object[]> NoValidationErrorsSublocationRecipients()
        {
            return
            [
                [
                    new SelectSublocationRecipientsModel
                    {
                        RenderedServiceRecipients =
                            [new SelectOption<string> { Value = "AAA", Selected = true }],
                    },
                ],
            ];
        }

        public static IEnumerable<object[]> ValidationErrorsSublocationRecipients()
        {
            return
            [
                [
                    new SelectSublocationRecipientsModel
                    {
                        RenderedServiceRecipients =
                            [new SelectOption<string> { Value = "AAA", Selected = false }],
                    },
                ],
            ];
        }
    }
}
