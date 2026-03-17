using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Orders;

public static class CallOffTermsDeclarationModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_WithMissingDeclaration_SetsModelError(
        CallOffTermsDeclarationModel model,
        CallOffTermsDeclarationModelValidator validator)
    {
        model.DeclarationAccepted = false;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.DeclarationAccepted)
            .WithErrorMessage(CallOffTermsDeclarationModelValidator.DeclarationRequiredErrorMessage);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_WithValidModel_NoModelError(
        CallOffTermsDeclarationModel model,
        CallOffTermsDeclarationModelValidator validator)
    {
        model.DeclarationAccepted = true;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(m => m.DeclarationAccepted);
    }
}
