using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed partial class Solution : IAudited
    {
        public Solution()
        {
            AdditionalServices = new HashSet<AdditionalService>();
            FrameworkSolutions = new HashSet<FrameworkSolution>();
            MarketingContacts = new HashSet<MarketingContact>();
            WorkOffPlans = new HashSet<WorkOffPlan>();
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Summary { get; set; }

        public string FullDescription { get; set; }

        public string Features { get; set; }

        public ClientApplication ClientApplication { get; set; }

        public Hosting Hosting { get; set; }

        public string ImplementationDetail { get; set; }

        public string RoadMap { get; set; }

        public string Integrations { get; set; }

        public string IntegrationsUrl { get; set; }

        public string AboutUrl { get; set; }

        public bool IsPilotSolution { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public ICollection<AdditionalService> AdditionalServices { get; set; }

        public ICollection<FrameworkSolution> FrameworkSolutions { get; set; }

        public ICollection<MarketingContact> MarketingContacts { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }

        public ICollection<WorkOffPlan> WorkOffPlans { get; set; }
    }
}
