using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class AddSolutionModelValidator : AbstractValidator<AddSolutionModel>
    {
        private readonly ISolutionsService solutionsService;

        public AddSolutionModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(s => s.SolutionName)
                .NotEmpty()
                .WithMessage("Enter a solution name");

            RuleFor(s => s.SupplierId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Select a supplier name");

            RuleFor(s => s.Frameworks)
                .Must(frameworks => frameworks.Any(f => f.Selected))
                .OverridePropertyName($"{nameof(AddSolutionModel.Frameworks)}[0].Selected")
                .WithMessage("Select the framework(s) your solution is available from");

            When(
                s => s.SupplierId.HasValue && !string.IsNullOrWhiteSpace(s.SolutionName),
                () =>
                {
                    RuleFor(s => s.SolutionName)
                        .Cascade(CascadeMode.Stop)
                        .MaximumLength(255)
                        .WithMessage("Solution name cannot be more than 255 characters")
                        .MustAsync(NotExistForSupplier)
                        .WithMessage("Solution name already exists. Enter a different solution name");
                });
        }

        private async Task<bool> NotExistForSupplier(
            AddSolutionModel model,
            string solutionName,
            CancellationToken cancellationToken) =>
            !await solutionsService.SupplierHasSolutionName(model.SupplierId!.Value, solutionName);
    }
}
