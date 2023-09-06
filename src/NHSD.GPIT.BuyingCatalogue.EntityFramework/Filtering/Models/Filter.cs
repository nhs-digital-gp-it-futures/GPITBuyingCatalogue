﻿using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class Filter : IAudited
    {
        public Filter()
        {
            Capabilities = new HashSet<Capability>();
            FilterHostingTypes = new HashSet<FilterHostingType>();
            FilterApplicationTypes = new HashSet<FilterApplicationType>();
            FilterCapabilityEpics = new HashSet<FilterCapabilityEpic>();
            FilterInteropIntegrationTypes = new HashSet<FilterInteroperabilityIntegrationType>();
            FilterIM1IntegrationsTypes = new HashSet<FilterIM1IntegrationsType>();
            FilterGPConnectIntegrationsTypes = new HashSet<FilterGPConnectIntegrationsType>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int OrganisationId { get; set; }

        public string FrameworkId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Organisation Organisation { get; set; }

        public Framework Framework { get; set; }

        public ICollection<Capability> Capabilities { get; set; }

        public ICollection<FilterCapabilityEpic> FilterCapabilityEpics { get; set; }

        public ICollection<FilterHostingType> FilterHostingTypes { get; set; }

        public ICollection<FilterApplicationType> FilterApplicationTypes { get; set; }

        public ICollection<FilterInteroperabilityIntegrationType> FilterInteropIntegrationTypes { get; set; }

        public ICollection<FilterIM1IntegrationsType> FilterIM1IntegrationsTypes { get; set; }

        public ICollection<FilterGPConnectIntegrationsType> FilterGPConnectIntegrationsTypes { get; set; }
    }
}
