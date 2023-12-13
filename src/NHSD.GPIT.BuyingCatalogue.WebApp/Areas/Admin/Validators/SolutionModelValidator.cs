using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class SolutionModelValidator : AbstractValidator<SolutionModel>
    {
        private readonly ISolutionsService solutionsService;

        public SolutionModelValidator(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(s => s.SolutionName)
                .NotEmpty()
                .WithMessage("Enter a solution name");

            RuleFor(s => s)
                .Must(NotBeADuplicateName)
                .WithMessage("A solution with this name already exists")
                .OverridePropertyName(m => m.SolutionName);

            RuleFor(s => s.SupplierId)
                .NotNull()
                .WithMessage("Select a supplier name");

            RuleFor(s => s.Frameworks)
                .Must(frameworks => frameworks.Any(f => f.Selected))
                .OverridePropertyName($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithMessage("Select the framework(s) your solution is available from");

            RuleFor(s => s.SolutionName)
                .MaximumLength(255)
                .WithMessage("Solution name cannot be more than 255 characters")
                .When(s => s.SupplierId.HasValue && !string.IsNullOrWhiteSpace(s.SolutionName));

            RuleFor(s => s.Frameworks)
                .Must(framework => framework.Count(x => x.IsFoundation && x.Selected) <= 1)
                .OverridePropertyName($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithMessage("A solution can only be marked as a Foundation Solution under one Framework at any given time");
        }

        private bool NotBeADuplicateName(SolutionModel model)
        {
            return !(model.SolutionId is null
                ? solutionsService.CatalogueSolutionExistsWithName(model.SolutionName).GetAwaiter().GetResult()
                : solutionsService.CatalogueSolutionExistsWithName(model.SolutionName, model.SolutionId!.Value).GetAwaiter().GetResult());
        }
    }
}
