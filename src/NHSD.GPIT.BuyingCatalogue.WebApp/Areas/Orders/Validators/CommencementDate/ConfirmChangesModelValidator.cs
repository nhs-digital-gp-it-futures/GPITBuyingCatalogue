using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.CommencementDate
{
    public class ConfirmChangesModelValidator : AbstractValidator<ConfirmChangesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select yes if you want to confirm changes to the commencement date";

        public ConfirmChangesModelValidator()
        {
            RuleFor(x => x.ConfirmChanges)
                .NotNull()
                .WithMessage(NoSelectionMadeErrorMessage);
        }
    }
}
