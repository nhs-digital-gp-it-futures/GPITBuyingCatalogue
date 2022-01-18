using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices
{
    public class EditAdditionalServiceModelValidator : AbstractValidator<EditAdditionalServiceModel>
    {
        public EditAdditionalServiceModelValidator()
        {
            RuleFor(m => m.OrderItem)
                .SetValidator(new CreateOrderItemModelValidator());
        }
    }
}
