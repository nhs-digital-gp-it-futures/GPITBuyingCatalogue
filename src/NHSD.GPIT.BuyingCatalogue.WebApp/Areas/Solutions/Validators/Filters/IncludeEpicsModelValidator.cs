using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Validators.Filters
{
    public class IncludeEpicsModelValidator : AbstractValidator<IncludeEpicsModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select if you want to filter by Epics or go to results";

        public IncludeEpicsModelValidator()
        {
            RuleFor(x => x.IncludeEpics)
                .NotNull()
                .WithMessage(NoSelectionMadeErrorMessage);
        }
    }
}
