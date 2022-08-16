using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.Filters
{
    public class FilterEpicsModelValidator : AbstractValidator<FilterEpicsModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select an Epic";

        public FilterEpicsModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveMadeASelection)
                .WithMessage(NoSelectionMadeErrorMessage);
        }

        private static bool HaveMadeASelection(FilterEpicsModel model) => model.SelectedItems?.Any(x => x.Selected) ?? false;
    }
}
