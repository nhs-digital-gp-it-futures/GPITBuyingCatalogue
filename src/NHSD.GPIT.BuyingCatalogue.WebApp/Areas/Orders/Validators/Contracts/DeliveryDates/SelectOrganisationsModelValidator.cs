using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;

public class SelectOrganisationsModelValidator : AbstractValidator<SelectOrganisationsModel>
{
    public const string RecipientsRequiredErrorMessage = "Select at least one organisation to change the delivery date";

    public SelectOrganisationsModelValidator()
    {
        RuleFor(x => x.Recipients)
            .Must(recipients => recipients?.Any(x => x.Selected) == true)
            .WithMessage(RecipientsRequiredErrorMessage);
    }
}
