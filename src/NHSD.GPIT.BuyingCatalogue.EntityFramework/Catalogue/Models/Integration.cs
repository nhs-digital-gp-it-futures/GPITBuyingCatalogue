using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class Integration
{
    public SupportedIntegrations Id { get; set; }

    public string Name { get; set; }

    public bool RequiresDescription => Id is SupportedIntegrations.NhsApp;

    public ICollection<IntegrationType> IntegrationTypes { get; set;  } = new HashSet<IntegrationType>();
}
