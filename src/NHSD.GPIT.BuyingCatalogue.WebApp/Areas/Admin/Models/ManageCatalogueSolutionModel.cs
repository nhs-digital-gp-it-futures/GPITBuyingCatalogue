using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public ManageCatalogueSolutionModel()
        {
        }

        public ManageCatalogueSolutionModel(CatalogueItem solution, IReadOnlyList<CatalogueItem> additionalServices, IReadOnlyList<CatalogueItem> associatedServices)
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            SelectedPublicationStatus = SolutionPublicationStatus = solution.PublishedStatus;
            SupplierName = solution.Supplier.Name;
            LastUpdated = solution.Solution.LastUpdated;

            var lastUpdatedBy = solution.Solution.LastUpdatedByUser;
            if (lastUpdatedBy is not null)
                LastUpdatedByName = $"{lastUpdatedBy.FirstName} {lastUpdatedBy.LastName}";

            DescriptionStatus = new DescriptionModel(solution).Status();
            FeaturesStatus = new FeaturesModel(solution).Status();
            ImplementationStatus = new ImplementationTimescaleModel(solution).Status();
            RoadmapStatus = new DevelopmentPlanModel(solution).Status();
            HostingTypeStatus = new HostingTypeSectionModel(solution).Status();
            ClientApplicationTypeStatus = new ClientApplicationTypeSectionModel(solution).Status();
            InteroperabilityStatus = new InteroperabilityModels.InteroperabilityModel(solution).Status();
            ListPriceStatus = new ListPriceModels.ManageListPricesModel(solution).Status();
            AssociatedServicesStatus = new AssociatedServicesModel(solution, associatedServices).Status();
            AdditionalServicesStatus = new AdditionalServicesModel(solution, additionalServices).Status();
            SupplierDetailsStatus = new EditSupplierDetailsModel(solution).Status();
            SlaStatus = new EditServiceLevelAgreementModel(solution).Status();
            CapabilitiesStatus = solution.CatalogueItemCapabilities.Any()
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
        }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }

        public PublicationStatus SolutionPublicationStatus { get; init; }

        public string SupplierName { get; init; }

        public DateTime LastUpdated { get; init; }

        public string LastUpdatedByName { get; init; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public IReadOnlyList<SelectListItem> PublicationStatuses => SolutionPublicationStatus
                .GetAvailablePublicationStatuses(CatalogueItemType.Solution)
                .Select(p => new SelectListItem(p.Description(), p.EnumMemberName()))
                .ToList();

        public TaskProgress DescriptionStatus { get; init; }

        public TaskProgress FeaturesStatus { get; init; }

        public TaskProgress ImplementationStatus { get; init; }

        public TaskProgress RoadmapStatus { get; init; }

        public TaskProgress HostingTypeStatus { get; init; }

        public TaskProgress ClientApplicationTypeStatus { get; init; }

        public TaskProgress InteroperabilityStatus { get; init; }

        public TaskProgress ListPriceStatus { get; init; }

        public TaskProgress AssociatedServicesStatus { get; init; }

        public TaskProgress AdditionalServicesStatus { get; init; }

        public TaskProgress SupplierDetailsStatus { get; init; }

        public TaskProgress SlaStatus { get; init; }

        public TaskProgress CapabilitiesStatus { get; init; }
    }
}
