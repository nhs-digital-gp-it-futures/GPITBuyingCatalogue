using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.ManageFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Validators.ManageFilters
{
    public static class SaveFilterModelValidatorTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SaveFilterModelValidator).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_NameNullOrEmpty_SetsModelError(
            string name,
            SaveFilterModel model,
            SaveFilterModelValidator validator)
        {
            model.Name = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(SaveFilterModelValidator.NameRequiredErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            SaveFilterModel model,
            SaveFilterModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage(SaveFilterModelValidator.DescriptionRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_DuplicateName_SetsModelError(
            [Frozen] IManageFiltersService mockManageFiltersService,
            SaveFilterModel model,
            SaveFilterModelValidator validator)
        {
            mockManageFiltersService.FilterExists(model.Name, model.OrganisationId).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(SaveFilterModelValidator.DuplicateNameErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            [Frozen] IManageFiltersService mockManageFiltersService,
            SaveFilterModel model,
            SaveFilterModelValidator validator)
        {
            mockManageFiltersService.FilterExists(model.Name, model.OrganisationId).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
