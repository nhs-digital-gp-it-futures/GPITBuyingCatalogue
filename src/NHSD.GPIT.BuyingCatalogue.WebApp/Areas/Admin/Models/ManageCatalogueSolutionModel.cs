using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

        public FeatureCompletionStatus StatusDescription() => new DescriptionModel(Solution).StatusDescription();

        public FeatureCompletionStatus StatusFeatures() => new FeaturesModel().FromCatalogueItem(Solution).Status();

        public FeatureCompletionStatus StatusImplementation() => new ImplementationTimescaleModel(Solution).StatusImplementation();

        public FeatureCompletionStatus StatusRoadmap() => new RoadmapModel().FromCatalogueItem(Solution).Status();

        public FeatureCompletionStatus StatusHostingType() => new HostingTypeSectionModel(Solution).StatusHostingType();

        public FeatureCompletionStatus StatusClientApplicationType() => new ClientApplicationTypeSectionModel(Solution).StatusClientApplicationType();

        public FeatureCompletionStatus StatusInteroperability() => new InteroperabilityModels.InteroperabilityModel(Solution).Status();
    }
}
