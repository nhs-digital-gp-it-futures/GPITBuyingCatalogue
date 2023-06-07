using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class JustifySolutionsModelValidator : AbstractValidator<JustifySolutionsModel>
{
    public JustifySolutionsModelValidator()
    {
        RuleForEach(x => x.Solutions)
            .Cascade(CascadeMode.Continue)
            .SetValidator(new SolutionJustificationModelValidator());
    }
}
