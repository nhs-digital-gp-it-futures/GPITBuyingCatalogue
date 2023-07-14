using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.Filters
{
    /*public class FilterCapabilitiesModelValidator : AbstractValidator<FilterCapabilitiesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a Capability";

        public FilterCapabilitiesModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveMadeASelection)
                .WithMessage(NoSelectionMadeErrorMessage);
        }

        private static bool HaveMadeASelection(FilterCapabilitiesModel model) => model.SelectedItems?.Any(x => x.Selected) ?? false;
    }*/
}
