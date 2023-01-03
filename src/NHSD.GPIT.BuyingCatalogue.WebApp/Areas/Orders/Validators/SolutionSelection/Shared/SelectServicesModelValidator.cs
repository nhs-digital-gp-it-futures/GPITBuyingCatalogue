using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Shared
{
    public class SelectServicesModelValidator : AbstractValidator<SelectServicesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select an associated service";

        public SelectServicesModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveMadeASelection)
                .OverridePropertyName("Services[0].IsSelected")
                .WithMessage(NoSelectionMadeErrorMessage);
        }

        private static bool HaveMadeASelection(SelectServicesModel model)
        {
            return !model.AssociatedServicesOnly
                || (model.Services?.Any(x => x.IsSelected) ?? false);
        }
    }
}
