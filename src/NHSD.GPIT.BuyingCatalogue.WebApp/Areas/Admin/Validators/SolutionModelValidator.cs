using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class SolutionModelValidator : AbstractValidator<SolutionModel>
    {
        public const string SelectSupplierName = "Select a supplier name";
        public const string EnterSolutionName = "Enter a solution name";
        public const string SolutionNameNoMoreThan255Characters = "Solution name cannot be more than 255 characters";
        public const string SolutionNameAlreadyExists = "A solution with this name already exists";
        public const string SelectFrameworkSolutionIsAvailableFrom = "Select the framework(s) your solution is available from";

        private readonly ISolutionsService solutionsService;

        public SolutionModelValidator(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(s => s.SolutionName)
                .NotEmpty()
                .WithMessage(EnterSolutionName);

            RuleFor(s => s)
                .Must(NotBeADuplicateName)
                .WithMessage(SolutionNameAlreadyExists)
                .OverridePropertyName(m => m.SolutionName);

            RuleFor(s => s.SupplierId)
                .NotNull()
                .WithMessage(SelectSupplierName);

            RuleFor(s => s.Frameworks)
                .Must(frameworks => frameworks.Any(f => f.Selected))
                .OverridePropertyName($"{nameof(SolutionModel.Frameworks)}[0].Selected")
                .WithMessage(SelectFrameworkSolutionIsAvailableFrom);

            RuleFor(s => s.SolutionName)
                .MaximumLength(255)
                .WithMessage(SolutionNameNoMoreThan255Characters)
                .When(s => s.SupplierId.HasValue && !string.IsNullOrWhiteSpace(s.SolutionName));
        }

        private bool NotBeADuplicateName(SolutionModel model)
        {
            return !(model.SolutionId is null
                ? solutionsService.CatalogueSolutionExistsWithName(model.SolutionName).GetAwaiter().GetResult()
                : solutionsService.CatalogueSolutionExistsWithName(model.SolutionName, model.SolutionId!.Value).GetAwaiter().GetResult());
        }
    }
}
