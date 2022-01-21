using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices
{
    public sealed class SelectAssociatedServiceModelValidator : AbstractValidator<SelectAssociatedServiceModel>
    {
        public SelectAssociatedServiceModelValidator()
        {
            RuleFor(m => m.SelectedSolutionId)
                .NotNull()
                .WithMessage("Select an Associated Service");
        }
    }
}
