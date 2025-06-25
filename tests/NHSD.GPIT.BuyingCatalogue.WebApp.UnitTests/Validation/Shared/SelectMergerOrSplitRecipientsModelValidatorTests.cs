using System.Linq;
using FluentValidation.TestHelper;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared;

public static class SelectMergerOrSplitRecipientsModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoSelectionMade_ThrowsValidationError(
        SelectMergerOrSplitRecipientsModel model,
        SelectMergerOrSplitRecipientsModelValidator systemUnderTest)
    {
        model.GetServiceRecipients().ForEach(x => x.Selected = false);

        TestValidationResult<SelectMergerOrSplitRecipientsModel> result = systemUnderTest.TestValidate(model);

        result.ShouldHaveValidationErrorFor("SubLocations[0].ServiceRecipients[0].Selected")
            .WithErrorMessage(
                string.Format(
                    SelectMergerOrSplitRecipientsModelValidator.SelectAtLeastErrorMessage,
                    SelectMergerOrSplitRecipientsModel.SelectAtLeast));
    }

    [Theory]
    [MockAutoData]
    public static void Validate_AtLeastSelectionMade_ThrowsValidationError(
        SelectMergerOrSplitRecipientsModel model,
        SelectMergerOrSplitRecipientsModelValidator systemUnderTest)
    {
        model.GetServiceRecipients().ForEach(x => x.Selected = false);
        model.GetServiceRecipients().First().Selected = true;

        TestValidationResult<SelectMergerOrSplitRecipientsModel> result = systemUnderTest.TestValidate(model);

        result.ShouldHaveValidationErrorFor("SubLocations[0].ServiceRecipients[0].Selected")
            .WithErrorMessage(
                string.Format(
                    SelectMergerOrSplitRecipientsModelValidator.SelectAtLeastErrorMessage,
                    SelectMergerOrSplitRecipientsModel.SelectAtLeast));
    }

    [Theory]
    [MockAutoData]
    public static void Validate_SelectionMade_NoValidationErrors(
        SelectMergerOrSplitRecipientsModel model,
        SelectMergerOrSplitRecipientsModelValidator systemUnderTest)
    {
        model.GetServiceRecipients().ForEach(x => x.Selected = true);

        TestValidationResult<SelectMergerOrSplitRecipientsModel> result = systemUnderTest.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
