using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public CatalogueItem Solution { get; private set; }

        public IReadOnlyList<CatalogueItem> AssociatedServices { get; private set; }

        public IReadOnlyList<CatalogueItem> AdditionalServices { get; private set; }

        public IReadOnlyList<SelectListItem> PublicationStatuses { get; private set; }

        public CatalogueItemId SolutionId { get; set; }

        public string LastUpdatedByName { get; set; }

        public PublicationStatus SelectedPublicationStatus { get; set; }

        public TaskProgress DescriptionStatus() => new DescriptionModel(Solution).Status();

        public TaskProgress FeaturesStatus() => new FeaturesModel().FromCatalogueItem(Solution).Status();

        public TaskProgress ImplementationStatus() => new ImplementationTimescaleModel(Solution).Status();

        public TaskProgress RoadmapStatus() => new RoadmapModel().FromCatalogueItem(Solution).Status();

        public TaskProgress HostingTypeStatus() => new HostingTypeSectionModel(Solution).Status();

        public TaskProgress ClientApplicationTypeStatus() => new ClientApplicationTypeSectionModel(Solution).Status();

        public TaskProgress InteroperabilityStatus() => new InteroperabilityModels.InteroperabilityModel(Solution).Status();

        public TaskProgress ListPriceStatus() => new ListPriceModels.ManageListPricesModel(Solution).Status();

        public TaskProgress AssociatedServicesStatus() =>
            new AssociatedServicesModel(Solution, AssociatedServices).Status();

        public TaskProgress AdditionalServicesStatus() =>
            new AdditionalServicesModel(Solution, AdditionalServices).Status();

        public TaskProgress SupplierDetailsStatus() => new EditSupplierDetailsModel(Solution).Status();

        public TaskProgress SlaStatus() => new EditServiceLevelAgreementModel(Solution).Status();

        public TaskProgress CapabilitiesStatus() => Solution.CatalogueItemCapabilities.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;

        public ManageCatalogueSolutionModel WithSolution(CatalogueItem solution)
        {
            SolutionId = solution.Id;
            Solution = solution;
            SelectedPublicationStatus = Solution.PublishedStatus;
            PublicationStatuses = Solution
                .PublishedStatus
                .GetAvailablePublicationStatuses(solution.CatalogueItemType)
                .Select(p => new SelectListItem(p.Description(), p.EnumMemberName()))
                .ToList();

            return this;
        }

        public ManageCatalogueSolutionModel WithAssociatedServices(List<CatalogueItem> associatedServices)
        {
            AssociatedServices = associatedServices;

            return this;
        }

        public ManageCatalogueSolutionModel WithAdditionalServices(List<CatalogueItem> additionalServices)
        {
            AdditionalServices = additionalServices;

            return this;
        }
    }
}
