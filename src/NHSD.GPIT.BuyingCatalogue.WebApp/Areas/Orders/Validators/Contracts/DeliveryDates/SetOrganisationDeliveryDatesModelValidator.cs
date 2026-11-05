using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;

public class SetOrganisationDeliveryDatesModelValidator
    : AbstractValidator<SetOrganisationDeliveryDatesModel>
{
    public SetOrganisationDeliveryDatesModelValidator()
    {
        RuleFor(x => x.OrderItemsDeliveryDates)
            .NotEmpty();

        RuleForEach(x => x.OrderItemsDeliveryDates)
            .SetValidator(new OrderItemDeliveryDateValidator());
    }
}
