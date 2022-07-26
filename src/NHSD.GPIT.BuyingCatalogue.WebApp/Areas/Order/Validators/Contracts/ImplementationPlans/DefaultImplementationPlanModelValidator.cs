using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.ImplementationPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.ImplementationPlans
{
    public class DefaultImplementationPlanModelValidator : AbstractValidator<DefaultImplementationPlanModel>
    {
        public const string NoSelectionErrorMessage = "Select yes if you would like to proceed with these milestones";

        public DefaultImplementationPlanModelValidator()
        {
            RuleFor(x => x.UseDefaultMilestones)
                .NotNull()
                .WithMessage(NoSelectionErrorMessage);
        }
    }
}
