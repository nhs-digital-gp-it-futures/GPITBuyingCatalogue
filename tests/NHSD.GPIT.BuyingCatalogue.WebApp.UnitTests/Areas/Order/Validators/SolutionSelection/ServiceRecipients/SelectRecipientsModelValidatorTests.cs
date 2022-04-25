using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.ServiceRecipients
{
    public static class SelectRecipientsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectRecipientsModel model,
            SelectRecipientsModelValidator systemUnderTest)
        {
            model.ServiceRecipients.ForEach(x => x.Selected = false);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor("ServiceRecipients[0].Selected")
                .WithErrorMessage(SelectRecipientsModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectRecipientsModel model,
            SelectRecipientsModelValidator systemUnderTest)
        {
            model.ServiceRecipients.ForEach(x => x.Selected = false);
            model.ServiceRecipients.First().Selected = true;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
