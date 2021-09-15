using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class ManageCatalogueSolutionModel : NavBaseModel
    {
        public CatalogueItem Solution { get; set; }

        public string LastUpdatedByName { get; set; }

        public PublicationStatus SelectedOption { get; set; }

        public IList<PublicationStatus> PublicationStatuses { get; }
            = Enum.GetValues<PublicationStatus>().ToList();

        public TaskProgress DescriptionStatus() => new DescriptionModel(Solution).DescriptionStatus();

        public TaskProgress FeaturesStatus() => new FeaturesModel().FromCatalogueItem(Solution).Status();

        public TaskProgress ImplementationStatus() => new ImplementationTimescaleModel(Solution).ImplementationStatus();

        public TaskProgress RoadmapStatus() => new RoadmapModel().FromCatalogueItem(Solution).Status();

        public TaskProgress HostingTypeStatus() => new HostingTypeSectionModel(Solution).HostingTypeStatus();

        public TaskProgress ClientApplicationTypeStatus() => new ClientApplicationTypeSectionModel(Solution).ClientApplicationTypeStatus();

        public TaskProgress InteroperabilityStatus() => new InteroperabilityModels.InteroperabilityModel(Solution).Status();
    }
}
