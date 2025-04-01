using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class SelectSublocationsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NotApplicable_NoValidationErrors(
            SelectSublocationsModel model,
            SelectSublocationsModelValidator systemUnderTest)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            SelectSublocationsModel model,
            SelectSublocationsModelValidator systemUnderTest)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectSublocationsModel model,
            SelectSublocationsModelValidator systemUnderTest)
        {
            Assert.Fail("not implemented");
        }
    }
}
