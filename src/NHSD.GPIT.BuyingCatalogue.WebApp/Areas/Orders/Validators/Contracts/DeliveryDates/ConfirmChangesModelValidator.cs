using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates
{
    public class ConfirmChangesModelValidator : AbstractValidator<ConfirmChangesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select yes if you want to confirm your changes";

        public ConfirmChangesModelValidator()
        {
            RuleFor(x => x.ConfirmChanges)
                .NotNull()
                .WithMessage(NoSelectionMadeErrorMessage);
        }
    }
}
