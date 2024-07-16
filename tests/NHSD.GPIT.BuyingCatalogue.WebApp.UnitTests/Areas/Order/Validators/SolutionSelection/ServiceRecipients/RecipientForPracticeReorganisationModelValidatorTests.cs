using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.ServiceRecipients;

public static class RecipientForPracticeReorganisationModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoFileSpecified_SetsModelError(
        RecipientForPracticeReorganisationModel model,
        RecipientForPracticeReorganisationModelValidator validator)
    {
        model.SelectedOdsCode = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.SelectedOdsCode)
            .WithErrorMessage(RecipientForPracticeReorganisationModelValidator.ErrorMessage);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_ValidFile_NoModelErrors(
        RecipientForPracticeReorganisationModel model,
        RecipientForPracticeReorganisationModelValidator validator)
    {
        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
