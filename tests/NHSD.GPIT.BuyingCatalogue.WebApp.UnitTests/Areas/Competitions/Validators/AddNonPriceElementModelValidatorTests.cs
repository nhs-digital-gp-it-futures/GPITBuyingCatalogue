using System.Collections.Generic;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
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
    public static void Validate_NoSelectedNonPriceElement_SetsModelError(
        AddNonPriceElementModel model,
        AddNonPriceElementModelValidator validator)
    {
        model.AvailableNonPriceElements = new List<SelectOption<NonPriceElement>>
        {
            new(NonPriceElement.Implementation.EnumMemberName(), NonPriceElement.Implementation),
            new(NonPriceElement.Interoperability.EnumMemberName(), NonPriceElement.Interoperability),
            new(NonPriceElement.ServiceLevel.EnumMemberName(), NonPriceElement.ServiceLevel),
        };

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("AvailableNonPriceElements[0].Selected")
            .WithErrorMessage(AddNonPriceElementModelValidator.NoSelectionError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_Valid_NoModelErrors(
        AddNonPriceElementModel model,
        AddNonPriceElementModelValidator validator)
    {
        model.AvailableNonPriceElements = new List<SelectOption<NonPriceElement>>
        {
            new(NonPriceElement.Implementation.EnumMemberName(), NonPriceElement.Implementation),
            new(NonPriceElement.Interoperability.EnumMemberName(), NonPriceElement.Interoperability),
            new(NonPriceElement.ServiceLevel.EnumMemberName(), NonPriceElement.ServiceLevel, selected: true),
        };

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
