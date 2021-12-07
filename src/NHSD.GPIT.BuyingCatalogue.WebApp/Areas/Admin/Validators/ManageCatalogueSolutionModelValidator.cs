using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class ManageCatalogueSolutionModelValidator : AbstractValidator<ManageCatalogueSolutionModel>
    {
        private readonly ISolutionsService solutionsService;

        public ManageCatalogueSolutionModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(m => m)
                .MustAsync(HaveCompletedAllMandatorySections)
                .WithMessage("Complete all mandatory sections before publishing")
                .OverridePropertyName(m => m.SelectedPublicationStatus);
        }

        private async Task<bool> HaveCompletedAllMandatorySections(ManageCatalogueSolutionModel model, CancellationToken cancellationToken)
        {
            _ = cancellationToken;

            var solution = await solutionsService.GetSolutionThin(model.SolutionId);
            if (model.SelectedPublicationStatus != PublicationStatus.Published || model.SelectedPublicationStatus == solution.PublishedStatus)
                return true;

            var mandatoryTasksStatuses = new[]
            {
                model.DescriptionStatus,
                model.ClientApplicationTypeStatus,
                model.ListPriceStatus,
                model.SupplierDetailsStatus,
                model.CapabilitiesStatus,
                model.SlaStatus,
            };

            return mandatoryTasksStatuses.All(status => status == ServiceContracts.Enums.TaskProgress.Completed);
        }
    }
}
