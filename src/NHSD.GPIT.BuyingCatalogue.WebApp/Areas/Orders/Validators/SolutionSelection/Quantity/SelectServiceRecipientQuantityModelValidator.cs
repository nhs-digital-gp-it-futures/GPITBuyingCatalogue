using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity
{
    public class SelectServiceRecipientQuantityModelValidator : AbstractValidator<SelectServiceRecipientQuantityModel>
    {
        public SelectServiceRecipientQuantityModelValidator()
        {
            RuleForEach(x => x.ServiceRecipients).Cascade(CascadeMode.Continue).SetValidator(new ServiceRecipientQuantityModelValidator());
        }
    }
}
