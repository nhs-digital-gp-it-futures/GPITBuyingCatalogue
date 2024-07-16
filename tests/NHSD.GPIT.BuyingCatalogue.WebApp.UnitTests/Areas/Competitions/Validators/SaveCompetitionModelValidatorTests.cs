using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SaveCompetitionModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoName_SetsModelError(
        SaveCompetitionModel model,
        SaveCompetitionModelValidator validator)
    {
        model.Name = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(SaveCompetitionModelValidator.NameMissingError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_DuplicateName_SetsModelError(
        SaveCompetitionModel model,
        [Frozen] ICompetitionsService competitionsService,
        SaveCompetitionModelValidator validator)
    {
        competitionsService.Exists(Arg.Any<string>(), model.Name).Returns(true);

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(SaveCompetitionModelValidator.DuplicateNameError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_NoDescription_SetsModelError(
        SaveCompetitionModel model,
        SaveCompetitionModelValidator validator)
    {
        model.Description = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(SaveCompetitionModelValidator.DescriptionMissingError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        SaveCompetitionModel model,
        [Frozen] ICompetitionsService competitionsService,
        SaveCompetitionModelValidator validator)
    {
        competitionsService.Exists(Arg.Any<string>(), model.Name).Returns(false);

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
