using System.Linq;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class AddNonPriceElementModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_SingleNonPriceElement_NoModelErrors(
        AddNonPriceElementModel model,
        AddNonPriceElementModelValidator validator)
    {
        model.SelectedNonPriceElement = null;
        model.AvailableNonPriceElements = model.AvailableNonPriceElements.Take(1).ToList();

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NoSelectedNonPriceElement_SetsModelError(
        AddNonPriceElementModel model,
        AddNonPriceElementModelValidator validator)
    {
        model.SelectedNonPriceElement = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SelectedNonPriceElement)
            .WithErrorMessage(AddNonPriceElementModelValidator.NoSelectionError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoModelErrors(
        NonPriceElement element,
        AddNonPriceElementModel model,
        AddNonPriceElementModelValidator validator)
    {
        model.SelectedNonPriceElement = element;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
