using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class Integration
    {
        public Guid Id { get; set; }

        public string IntegrationType { get; set; }

        public string Qualifier { get; set; }

        public HashSet<string> NHSAppIntegrationTypes { get; set; }

        public bool IsConsumer { get; set; }

        public string IntegratesWith { get; set; }

        public string Description { get; set; }

        public string AdditionalInformation { get; set; }
    }
}
