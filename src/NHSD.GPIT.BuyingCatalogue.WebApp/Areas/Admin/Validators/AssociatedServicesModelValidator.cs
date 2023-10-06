using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class AssociatedServicesModelValidator : AbstractValidator<AssociatedServicesModel>
    {
        public AssociatedServicesModelValidator()
        {
            RuleFor(m => m.SolutionMergerAndSplits)
                .Must(x => x.IsValid)
                .WithMessage("The solution already has an Associated Service of this type, so you cannot add another")
                .OverridePropertyName("selectable-associated-services");
        }
    }
}
