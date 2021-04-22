using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution
{
    public class SolutionStatusModel : MarketingBaseModel
    {
        public SolutionStatusModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {            
        }                

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }

        public string SolutionDescriptionStatus
        {
            get { return GetStatus(new SolutionDescriptionModel(CatalogueItem)); }
        }

        public string FeaturesStatus
        {
            get { return GetStatus(new FeaturesModel(CatalogueItem)); }
        }

        public string IntegrationsStatus
        {
            get { return GetStatus(new IntegrationsModel(CatalogueItem)); }
        }

        public string ImplementationTimescalesStatus
        {
            get { return GetStatus(new ImplementationTimescalesModel(CatalogueItem)); }
        }

        public string RoadmapStatus
        {
            get { return GetStatus(new RoadmapModel(CatalogueItem)); }
        }

        public string ClientApplicationTypeStatus
        {
            get { return GetStatus(new ClientApplicationTypesModel(CatalogueItem)); }
        }

        public string AboutSupplierStatus
        {
            get { return GetStatus(new AboutSupplierModel(CatalogueItem)); }
        }

        public string ContactDetailsStatus
        {
            get { return GetStatus(new ContactDetailsModel(CatalogueItem)); }
        }
    }
}
