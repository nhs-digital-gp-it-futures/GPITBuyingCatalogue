using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Hosting
{
    public sealed class OnPremiseModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoLink_DoesNotValidate(
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            var model = new OnPremiseModel();

            var result = validator.TestValidate(model);

            urlValidator.Verify(uv => uv.IsValidUrl(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidLink_SetsModelError(
            OnPremiseModel model,
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a valid URL");
        }

        [Theory]
        [CommonAutoData]
        public static void VAlidate_ValidLink_NoModelError(
            OnPremiseModel model,
            [Frozen] Mock<IUrlValidator> urlValidator,
            OnPremiseModelValidator validator)
        {
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
