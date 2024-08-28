using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class ManageCatalogueSolutionModelValidator : AbstractValidator<ManageCatalogueSolutionModel>
    {
        private readonly ISolutionsService solutionsService;

        public ManageCatalogueSolutionModelValidator(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService;

            RuleFor(m => m)
                .Must(HaveCompletedAllMandatorySections)
                .WithMessage("Complete all mandatory sections before publishing")
                .OverridePropertyName(m => m.SelectedPublicationStatus);
        }

        private bool HaveCompletedAllMandatorySections(ManageCatalogueSolutionModel model)
        {
            var solution = solutionsService.GetSolutionThin(model.SolutionId).GetAwaiter().GetResult();
            if (model.SelectedPublicationStatus != PublicationStatus.Published || model.SelectedPublicationStatus == solution.PublishedStatus)
                return true;

            var mandatoryTasksStatuses = new[]
            {
                model.DescriptionStatus,
                model.ApplicationTypeStatus,
                model.ListPriceStatus,
                model.CapabilitiesStatus,
                model.SupplierDetailsStatus,
                model.SlaStatus,
            };

            return mandatoryTasksStatuses.All(status => status == ServiceContracts.Enums.TaskProgress.Completed);
        }
    }
}
