using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Organisation
{
    public static class AddAnOrganisationModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectedOrganisation_SetsModelError(
            AddAnOrganisationModel model,
            AddAnOrganisationModelValidator validator)
        {
            model.SelectedOrganisation = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisation)
                .WithErrorMessage("Select a related organisation");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedOrganisation_NoModelError(
            AddAnOrganisationModel model,
            AddAnOrganisationModelValidator validator)
        {
            model.SelectedOrganisation = 1;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
