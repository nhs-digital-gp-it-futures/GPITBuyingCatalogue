using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SaveFilterModelValidator).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
        [CommonAutoData]
        public static void Validate_DuplicateName_SetsModelError(
            [Frozen] Mock<IManageFiltersService> mockManageFiltersService,
            SaveFilterModel model,
            SaveFilterModelValidator validator)
        {
            mockManageFiltersService
                .Setup(x => x.FilterExists(model.Name, model.OrganisationId))
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(SaveFilterModelValidator.DuplicateNameErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            [Frozen] Mock<IManageFiltersService> mockManageFiltersService,
            SaveFilterModel model,
            SaveFilterModelValidator validator)
        {
            mockManageFiltersService
                .Setup(x => x.FilterExists(model.Name, model.OrganisationId))
                .ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
