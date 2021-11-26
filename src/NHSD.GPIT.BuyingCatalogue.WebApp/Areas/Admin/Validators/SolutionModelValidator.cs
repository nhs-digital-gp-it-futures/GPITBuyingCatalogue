using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

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
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Enter a solution name");

            RuleFor(s => s)
                .MustAsync(NotBeADuplicateName)
                .WithMessage("A solution with this name already exists")
                .OverridePropertyName(m => m.SolutionName);

            RuleFor(s => s.SupplierId)
                .Cascade(CascadeMode.Stop)
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
        }

        private async Task<bool> NotBeADuplicateName(SolutionModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            return !(model.SolutionId is null
                ? await solutionsService.CatalogueSolutionExistsWithName(model.SolutionName)
                : await solutionsService.CatalogueSolutionExistsWithName(model.SolutionName, model.SolutionId!.Value));
        }
    }
}
