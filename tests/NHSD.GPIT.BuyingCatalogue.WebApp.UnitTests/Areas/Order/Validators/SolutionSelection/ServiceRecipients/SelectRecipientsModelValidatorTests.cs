using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.ServiceRecipients;
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
            model.GetServiceRecipients().ForEach(x => x.Selected = false);

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor("SubLocations[0].ServiceRecipients[0].Selected")
                .WithErrorMessage(SelectRecipientsModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectRecipientsModel model,
            SelectRecipientsModelValidator systemUnderTest)
        {
            model.GetServiceRecipients().ForEach(x => x.Selected = false);
            model.GetServiceRecipients().First().Selected = true;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
