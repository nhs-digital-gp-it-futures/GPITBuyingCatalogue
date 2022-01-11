using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public sealed class EditSolutionModelValidator : AbstractValidator<EditSolutionModel>
    {
        public EditSolutionModelValidator()
        {
            RuleFor(m => m.OrderItem)
                .SetValidator(new CreateOrderItemModelValidator());
        }
    }
}
