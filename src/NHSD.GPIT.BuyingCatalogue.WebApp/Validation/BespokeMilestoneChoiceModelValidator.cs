using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

public class BespokeMilestoneChoiceModelValidator : AbstractValidator<BespokeMilestoneChoiceModel>
{
    public BespokeMilestoneChoiceModelValidator()
    {
        RuleFor(x => x.ShouldAddMilestone)
            .NotNull()
            .WithMessage("Make a selection");
    }
}
