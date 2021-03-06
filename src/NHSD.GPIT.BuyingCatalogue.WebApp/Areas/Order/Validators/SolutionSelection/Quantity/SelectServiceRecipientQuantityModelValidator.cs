using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Quantity
{
    public class SelectServiceRecipientQuantityModelValidator : AbstractValidator<SelectServiceRecipientQuantityModel>
    {
        public SelectServiceRecipientQuantityModelValidator()
        {
            RuleForEach(x => x.ServiceRecipients).SetValidator(new ServiceRecipientQuantityModelValidator());
        }
    }
}
