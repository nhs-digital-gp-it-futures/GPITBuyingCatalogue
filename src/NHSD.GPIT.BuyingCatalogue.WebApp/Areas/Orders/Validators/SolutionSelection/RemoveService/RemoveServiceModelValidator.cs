using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.CatalogueSolutions
{
    public class RemoveServiceModelValidator : AbstractValidator<RemoveServiceModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select yes if you want to remove {0}";

        public RemoveServiceModelValidator()
        {
            RuleFor(x => x.ConfirmRemoveService)
                .NotEmpty()
                .WithMessage(x => string.Format(NoSelectionMadeErrorMessage, x.ServiceName));
        }
    }
}
