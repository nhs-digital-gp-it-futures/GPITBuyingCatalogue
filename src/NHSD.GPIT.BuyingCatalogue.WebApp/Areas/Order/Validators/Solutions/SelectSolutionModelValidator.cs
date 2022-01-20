using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public sealed class SelectSolutionModelValidator : AbstractValidator<SelectSolutionModel>
    {
        public SelectSolutionModelValidator()
        {
            RuleFor(m => m.SelectedSolutionId)
                .NotNull()
                .WithMessage("Select a Catalogue Solution");
        }
    }
}
