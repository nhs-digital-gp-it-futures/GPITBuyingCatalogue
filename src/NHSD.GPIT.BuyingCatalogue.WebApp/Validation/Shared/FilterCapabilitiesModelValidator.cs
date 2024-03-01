using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared
{
    public class FilterCapabilitiesModelValidator : AbstractValidator<FilterCapabilitiesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a Capability";

        public FilterCapabilitiesModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveMadeASelection)
                .WithMessage(NoSelectionMadeErrorMessage)
                .When(m => !m.IsFilter);
        }

        private static bool HaveMadeASelection(FilterCapabilitiesModel model) => model.CapabilitySelectionItems?.Any(x => x.Selected) ?? false;
    }
}
