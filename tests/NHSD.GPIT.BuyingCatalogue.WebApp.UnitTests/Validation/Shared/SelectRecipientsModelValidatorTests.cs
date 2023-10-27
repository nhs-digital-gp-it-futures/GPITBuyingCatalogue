using System.Linq;
using FluentValidation.TestHelper;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared;

public static class SelectRecipientsModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NoSelectionMade_ThrowsValidationError(
        SelectRecipientsModel model,
        SelectRecipientsModelValidator systemUnderTest)
    {
        model.GetServiceRecipients().ForEach(x => x.Selected = false);

        var result = systemUnderTest.TestValidate(model);

        result.ShouldHaveValidationErrorFor("SubLocations[0].ServiceRecipients[0].Selected")
            .WithErrorMessage(SelectRecipientsModelValidator.NoSelectionMadeErrorMessage);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_AtLeastSelectionMade_ThrowsValidationError(
        SelectRecipientsModel model,
        SelectRecipientsModelValidator systemUnderTest)
    {
        model.SelectAtLeast = 2;
        model.GetServiceRecipients().ForEach(x => x.Selected = false);
        model.GetServiceRecipients().First().Selected = true;

        var result = systemUnderTest.TestValidate(model);

        result.ShouldHaveValidationErrorFor("SubLocations[0].ServiceRecipients[0].Selected")
            .WithErrorMessage(string.Format(SelectRecipientsModelValidator.SelectAtLeastErrorMessage, model.SelectAtLeast.Value));
    }

    [Theory]
    [CommonInlineAutoData(null)]
    [CommonInlineAutoData(0)]
    [CommonInlineAutoData(1)]
    public static void Validate_SelectionMade_NoValidationErrors(
        int? selectAtLeast,
        SelectRecipientsModel model,
        SelectRecipientsModelValidator systemUnderTest)
    {
        model.SelectAtLeast = selectAtLeast;
        model.GetServiceRecipients().ForEach(x => x.Selected = false);
        model.GetServiceRecipients().First().Selected = true;

        var result = systemUnderTest.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
