using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
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

        private static List<TaskProgress> GetMandatoryTaskStatuses(ManageCatalogueSolutionModel model)
        {
            if (model.SolutionSections is null)
                return [];

            (SolutionSectionRequirement Requirement, TaskProgress Status)[] tasks =
            [
                (model.SolutionSections.Description, model.DescriptionStatus),
                (model.SolutionSections.Features, model.FeaturesStatus),
                (model.SolutionSections.Integrations, model.InteroperabilityStatus),
                (model.SolutionSections.Implementation, model.ImplementationStatus),
                (model.SolutionSections.ApplicationType, model.ApplicationTypeStatus),
                (model.SolutionSections.HostingType, model.HostingTypeStatus),
                (model.SolutionSections.DataProcessingInformation, model.DataProcessingStatus),
                (model.SolutionSections.ListPrice, model.ListPriceStatus),
                (model.SolutionSections.CapabilitiesAndEpics, model.CapabilitiesStatus),
                (model.SolutionSections.AdditionalServices, model.AdditionalServicesStatus),
                (model.SolutionSections.AssociatedServices, model.AssociatedServicesStatus),
                (model.SolutionSections.Standards, model.StandardsStatus),
                (model.SolutionSections.DevelopmentPlans, model.RoadmapStatus),
                (model.SolutionSections.SupplierDetails, model.SupplierDetailsStatus),
                (model.SolutionSections.ServiceLevelAgreement, model.SlaStatus),
            ];

            return [.. tasks.Where(task => task.Requirement == SolutionSectionRequirement.Mandatory).Select(task => task.Status)];
        }

        private bool HaveCompletedAllMandatorySections(ManageCatalogueSolutionModel model)
        {
            var solution = solutionsService.GetSolutionThin(model.SolutionId).GetAwaiter().GetResult();
            if (model.SelectedPublicationStatus != PublicationStatus.Published || model.SelectedPublicationStatus == solution.PublishedStatus)
                return true;

            var mandatoryTasksStatuses = GetMandatoryTaskStatuses(model);

            return mandatoryTasksStatuses.All(status => status == TaskProgress.Completed);
        }
    }
}
