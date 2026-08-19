using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation
{
    public static class BespokeMilestoneChoiceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelection_SetsModelError(
            BespokeMilestoneChoiceModel model,
            BespokeMilestoneChoiceModelValidator validator)
        {
            model.ShouldAddMilestone = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ShouldAddMilestone)
                .WithErrorMessage("Make a selection");
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Validate_SelectionMade_NoModelError(
            bool shouldAddMilestone,
            BespokeMilestoneChoiceModel model,
            BespokeMilestoneChoiceModelValidator validator)
        {
            model.ShouldAddMilestone = shouldAddMilestone;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.ShouldAddMilestone);
        }
    }
}
