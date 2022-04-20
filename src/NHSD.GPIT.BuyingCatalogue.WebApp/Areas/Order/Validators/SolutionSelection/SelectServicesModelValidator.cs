using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection
{
    public class SelectServicesModelValidator : AbstractValidator<SelectServicesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a service";

        public SelectServicesModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveMadeASelection)
                .OverridePropertyName("Services[0].IsSelected")
                .WithMessage(NoSelectionMadeErrorMessage);
        }

        private static bool HaveMadeASelection(SelectServicesModel model)
        {
            if (!model.AssociatedServicesOnly)
            {
                return true;
            }

            return model.ExistingServices?.Any() ?? model.Services.Any(x => x.IsSelected);
        }
    }
}
