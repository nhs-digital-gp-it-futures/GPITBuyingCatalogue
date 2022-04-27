using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Solutions
{
    public static class SelectSolutionServiceRecipientsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoRecipientsSelected_SetsModelError(
            SelectSolutionServiceRecipientsModel model,
            SelectSolutionServiceRecipientsModelValidator validator)
        {
            model.ServiceRecipients.ForEach(sr => sr.Selected = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("ServiceRecipients[0].Selected")
                .WithErrorMessage("Select a Service Recipient");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_RecipientSelected_NoModelError(
            SelectSolutionServiceRecipientsModel model,
            SelectSolutionServiceRecipientsModelValidator validator)
        {
            model.ServiceRecipients[0].Selected = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
