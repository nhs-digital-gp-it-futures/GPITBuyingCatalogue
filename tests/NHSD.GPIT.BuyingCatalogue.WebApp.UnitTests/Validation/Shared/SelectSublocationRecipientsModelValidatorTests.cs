using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class SelectSublocationRecipientsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NotApplicable_NoValidationErrors(
            SelectSublocationRecipientsModel model,
            SelectSublocationRecipientsModelValidator systemUnderTest)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectSublocationRecipientsModel model,
            SelectSublocationRecipientsModelValidator systemUnderTest)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectSublocationRecipientsModel model,
            SelectSublocationRecipientsModelValidator systemUnderTest)
        {
            Assert.Fail("not implemented");
        }
    }
}
