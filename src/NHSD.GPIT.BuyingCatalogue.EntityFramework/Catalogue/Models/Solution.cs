using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Solution
    {
        public Solution()
        {
            AdditionalServices = new HashSet<AdditionalService>();
            FrameworkSolutions = new HashSet<FrameworkSolution>();
            MarketingContacts = new HashSet<MarketingContact>();
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Version { get; set; }

        public string Summary { get; set; }

        public string FullDescription { get; set; }

        public string Features { get; set; }

        public string ClientApplication { get; set; }

        public string Hosting { get; set; }

        public string ImplementationDetail { get; set; }

        public string RoadMap { get; set; }

        public string Integrations { get; set; }

        public string IntegrationsUrl { get; set; }

        public string AboutUrl { get; set; }

        public string ServiceLevelAgreement { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public ICollection<AdditionalService> AdditionalServices { get; set; }

        public ICollection<FrameworkSolution> FrameworkSolutions { get; set; }

        public ICollection<MarketingContact> MarketingContacts { get; set; }
    }
}
