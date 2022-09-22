using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.DeliveryDates
{
    public class EditDatesModelValidator : AbstractValidator<EditDatesModel>
    {
        public EditDatesModelValidator()
        {
            RuleForEach(x => x.Recipients).SetValidator(new RecipientDateModelValidator());
        }
    }
}
