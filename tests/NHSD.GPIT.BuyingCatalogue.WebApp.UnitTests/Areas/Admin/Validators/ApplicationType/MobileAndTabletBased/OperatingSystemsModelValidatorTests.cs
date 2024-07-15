using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.MobileTabletBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.MobileAndTabletBased
{
    public static class OperatingSystemsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectedOperatingSystems_SetsModelError(
            OperatingSystemsModel model,
            OperatingSystemsModelValidator validator)
        {
            model.OperatingSystems.ToList().ForEach(os => os.Checked = false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("OperatingSystems[0].Checked")
                .WithErrorMessage("Select at least one supported operating system");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedOperatingSystem_NoModelError(
            OperatingSystemsModel model,
            OperatingSystemsModelValidator validator)
        {
            model.OperatingSystems[0].Checked = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
