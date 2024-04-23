using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration
{
    public class ManagePasswordModelValidator : AbstractValidator<ManagePasswordModel>
    {
        public ManagePasswordModelValidator(IValidator<UpdatePasswordViewModel> validator)
        {
            RuleFor(customer => customer.UpdatePasswordViewModel).SetValidator(validator);
        }
    }
}
