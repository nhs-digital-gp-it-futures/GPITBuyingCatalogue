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

            RuleFor(s => s.FrameworkModel)
                .Must(HaveValidFramework)
                .WithMessage("Select the framework(s) your solution is available from");

            RuleFor(s => s.SupplierId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Select a supplier name")
                .Must(s => int.TryParse(s, out _))
                .WithMessage("Supplier Id should be a valid integer");

            RuleFor(s => s.SolutionName)
                .NotEmpty()
                .WithMessage("Enter a solution name");

            When(
                s => int.TryParse(s.SupplierId, out _) && !string.IsNullOrWhiteSpace(s.SolutionName),
                () =>
                {
                    RuleFor(s => s.SolutionName)
                        .Cascade(CascadeMode.Stop)
                        .MaximumLength(255)
                        .WithMessage($"Solution name cannot be more than 255 characters")
                        .MustAsync(NotExistForSupplier)
                        .WithMessage("Solution name already exists. Enter a different solution name");
                });
        }

        private static bool HaveValidFramework(FrameworkModel frameworkModel) =>
            frameworkModel?.IsValid() == true;

        private async Task<bool> NotExistForSupplier(
            AddSolutionModel model,
            string solutionName,
            CancellationToken arg3) =>
            !(await solutionsService.SupplierHasSolutionName(model.SupplierId, solutionName));
    }
}
