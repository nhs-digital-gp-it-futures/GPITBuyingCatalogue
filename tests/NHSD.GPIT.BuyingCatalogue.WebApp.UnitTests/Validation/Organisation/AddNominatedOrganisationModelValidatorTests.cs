using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Organisation
{
    public static class AddNominatedOrganisationModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            AddNominatedOrganisationModel model,
            AddNominatedOrganisationModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
