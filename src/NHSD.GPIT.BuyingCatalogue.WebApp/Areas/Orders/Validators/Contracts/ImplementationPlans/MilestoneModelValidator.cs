using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.ImplementationPlans
{
    public class MilestoneModelValidator : AbstractValidator<MilestoneModel>
    {
        public const string NameRequiredErrorMessage = "Enter a milestone name";
        public const string PaymentTriggerRequiredErrorMessage = "Enter a milestone payment trigger";

        public MilestoneModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(NameRequiredErrorMessage);

            RuleFor(x => x.PaymentTrigger)
                .NotEmpty()
                .WithMessage(PaymentTriggerRequiredErrorMessage);
        }
    }
}
