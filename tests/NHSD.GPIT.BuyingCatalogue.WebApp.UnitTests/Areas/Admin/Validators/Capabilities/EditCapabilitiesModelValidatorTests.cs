using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Capabilities;

public static class EditCapabilitiesModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoSelectedCapabilities_SetsModelError(
        EditCapabilitiesModel model,
        EditCapabilitiesModelValidator validator)
    {
        model.CapabilityCategories.ForEach(cc => cc.Capabilities.ForEach(c => c.Selected = false));

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(EditCapabilitiesModelValidator.EditCapabilitiesPropertyName)
            .WithErrorMessage(EditCapabilitiesModelValidator.NoSelectedCapabilitiesError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_CapabilitiesSelected_NoModelError(
        EditCapabilitiesModel model,
        EditCapabilitiesModelValidator validator)
    {
        model.CapabilityCategories.ForEach(cc => cc.Capabilities.ForEach(c => c.Selected = true));

        var result = validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(EditCapabilitiesModelValidator.EditCapabilitiesPropertyName);
    }
}
