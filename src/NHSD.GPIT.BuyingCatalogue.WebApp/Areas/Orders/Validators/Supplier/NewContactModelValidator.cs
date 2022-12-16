using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier
{
    public class NewContactModelValidator : AbstractValidator<NewContactModel>
    {
        public NewContactModelValidator()
        {
            Include(new ContactModelValidator());
        }
    }
}
