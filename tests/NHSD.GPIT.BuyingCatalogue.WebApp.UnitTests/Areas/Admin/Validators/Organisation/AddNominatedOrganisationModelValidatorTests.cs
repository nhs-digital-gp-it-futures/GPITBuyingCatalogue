using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Organisation
{
    public static class AddNominatedOrganisationModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_SelectedOrganisationIdNullOrEmpty_SetsModelError(
            string selectedOrganisationId,
            AddNominatedOrganisationModel model,
            AddNominatedOrganisationModelValidator validator)
        {
            model.SelectedOrganisationId = selectedOrganisationId;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrganisationId)
                .WithErrorMessage(AddNominatedOrganisationModelValidator.OrganisationMissingError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            AddNominatedOrganisationModel model,
            AddNominatedOrganisationModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
