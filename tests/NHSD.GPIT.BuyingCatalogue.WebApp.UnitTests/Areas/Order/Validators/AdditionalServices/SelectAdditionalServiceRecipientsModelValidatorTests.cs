using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AdditionalServices
{
    public static class SelectAdditionalServiceRecipientsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoRecipientsSelected_SetsModelError(
            SelectAdditionalServiceRecipientsModel model,
            SelectAdditionalServiceRecipientsModelValidator validator)
        {
            model.ServiceRecipients.ForEach(sr => sr.Selected = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("ServiceRecipients[0].Selected")
                .WithErrorMessage("Select a Service Recipient");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_RecipientSelected_NoModelError(
            SelectAdditionalServiceRecipientsModel model,
            SelectAdditionalServiceRecipientsModelValidator validator)
        {
            model.ServiceRecipients[0].Selected = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
