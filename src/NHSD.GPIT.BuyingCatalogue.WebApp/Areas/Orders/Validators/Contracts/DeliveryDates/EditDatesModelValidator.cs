using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    public class EditDatesModelValidator : AbstractValidator<EditDatesModel>
    {
        public EditDatesModelValidator()
        {
            RuleForEach(x => x.Recipients)
                .Cascade(CascadeMode.Continue)
                .SetValidator(new RecipientLocationModelValidator());
        }
    }
}
