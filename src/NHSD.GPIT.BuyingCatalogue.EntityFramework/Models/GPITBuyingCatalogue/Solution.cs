using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class Solution
    {
        public Solution()
        {
            AdditionalServices = new HashSet<AdditionalService>();
            FrameworkSolutions = new HashSet<FrameworkSolution>();
            MarketingContacts = new HashSet<MarketingContact>();
            SolutionCapabilities = new HashSet<SolutionCapability>();
            SolutionEpics = new HashSet<SolutionEpic>();
        }

        public string Id { get; set; }

        public string Version { get; set; }

        public string Summary { get; set; }

        public string FullDescription { get; set; }

        public string Features { get; set; }

        public string ClientApplication { get; set; }

        public string Hosting { get; set; }

        public string ImplementationDetail { get; set; }

        public string RoadMap { get; set; }

        public string IntegrationsUrl { get; set; }

        public string AboutUrl { get; set; }

        public string ServiceLevelAgreement { get; set; }

        public string WorkOfPlan { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public virtual CatalogueItem IdNavigation { get; set; }

        public virtual ICollection<AdditionalService> AdditionalServices { get; set; }

        public virtual ICollection<FrameworkSolution> FrameworkSolutions { get; set; }

        public virtual ICollection<MarketingContact> MarketingContacts { get; set; }

        public virtual ICollection<SolutionCapability> SolutionCapabilities { get; set; }

        public virtual ICollection<SolutionEpic> SolutionEpics { get; set; }
    }
}
