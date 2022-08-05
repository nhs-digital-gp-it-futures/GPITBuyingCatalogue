using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier
{
    public class NewContactModelValidator : AbstractValidator<NewContactModel>
    {
        public NewContactModelValidator()
        {
            Include(new ContactModelValidator());
        }
    }
}
