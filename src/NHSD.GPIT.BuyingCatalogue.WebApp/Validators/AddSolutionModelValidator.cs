using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validators
{
    public sealed class AddSolutionModelValidator : AbstractValidator<AddSolutionModel>
    {
        private readonly ISolutionsService solutionsService;

        public AddSolutionModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(s => s.FrameworkModel)
                .Must(HaveFrameworkSelected)
                .WithMessage("Select the framework(s) your solution is available from");

            RuleFor(s => s)
                .MustAsync(NotExistForSupplier)
                .WithMessage("Solution name already exists. Enter a different solution name");
        }

        private static bool HaveFrameworkSelected(FrameworkModel frameworkModel) => frameworkModel != null
            && (frameworkModel.DfocvcFramework || frameworkModel.GpitFuturesFramework);

        private async Task<bool> NotExistForSupplier(AddSolutionModel model, CancellationToken arg2) =>
            !(await solutionsService.SupplierHasSolutionName(model.SupplierId, model.SolutionName));
    }
}
