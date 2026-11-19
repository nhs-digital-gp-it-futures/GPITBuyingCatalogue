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
            List<TaskProgress> mandatoryTasksStatuses = [];

            if (model.SolutionSections is null)
                return mandatoryTasksStatuses;

            if (model.SolutionSections.Description == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.DescriptionStatus);

            if (model.SolutionSections.Features == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.FeaturesStatus);

            if (model.SolutionSections.Integrations == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.InteroperabilityStatus);

            if (model.SolutionSections.Implementation == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.ImplementationStatus);

            if (model.SolutionSections.ApplicationType == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.ApplicationTypeStatus);

            if (model.SolutionSections.HostingType == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.HostingTypeStatus);

            if (model.SolutionSections.DataProcessingInformation == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.DataProcessingStatus);

            if (model.SolutionSections.ListPrice == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.ListPriceStatus);

            if (model.SolutionSections.CapabilitiesAndEpics == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.CapabilitiesStatus);

            if (model.SolutionSections.AdditionalServices == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.AdditionalServicesStatus);

            if (model.SolutionSections.AssociatedServices == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.AssociatedServicesStatus);

            if (model.SolutionSections.Standards == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.StandardsStatus);

            if (model.SolutionSections.DevelopmentPlans == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.RoadmapStatus);

            if (model.SolutionSections.SupplierDetails == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.SupplierDetailsStatus);

            if (model.SolutionSections.ServiceLevelAgreement == SolutionSectionRequirement.Mandatory)
                mandatoryTasksStatuses.Add(model.SlaStatus);

            return mandatoryTasksStatuses;
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
