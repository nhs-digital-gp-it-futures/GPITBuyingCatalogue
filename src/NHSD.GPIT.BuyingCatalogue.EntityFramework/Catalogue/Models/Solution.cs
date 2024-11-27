using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed partial class Solution : IAudited
    {
        public CatalogueItemId CatalogueItemId { get; set; }

        public SolutionCategory? Category { get; set; }

        public string Summary { get; set; }

        public string FullDescription { get; set; }

        public string Features { get; set; }

        public ApplicationTypeDetail ApplicationTypeDetail { get; set; }

        public Hosting Hosting { get; set; }

        public string ImplementationDetail { get; set; }

        public string RoadMap { get; set; }

        public string IntegrationsUrl { get; set; }

        public string AboutUrl { get; set; }

        public bool IsPilotSolution { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }

        public DataProcessingInformation DataProcessingInformation { get; set; }

        public ICollection<AdditionalService> AdditionalServices { get; set; } = new HashSet<AdditionalService>();

        public ICollection<FrameworkSolution> FrameworkSolutions { get; set; } = new HashSet<FrameworkSolution>();

        public ICollection<MarketingContact> MarketingContacts { get; set; } = new HashSet<MarketingContact>();

        public ICollection<SolutionIntegration> Integrations { get; set; } = new HashSet<SolutionIntegration>();

        public ICollection<WorkOffPlan> WorkOffPlans { get; set; } = new HashSet<WorkOffPlan>();
    }
}
