using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class IntegrationType
{
    public int Id { get; set; }

    public SupportedIntegrations IntegrationId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Integration Integration { get; set; }

    public ICollection<Solution> Solutions { get; set; } = new HashSet<Solution>();
}
