using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

public sealed class FilterIntegration
{
    public FilterIntegration()
    {
    }

    public FilterIntegration(SupportedIntegrations integrationId)
    {
        IntegrationId = integrationId;
    }

    public int FilterId { get; set; }

    public SupportedIntegrations IntegrationId { get; set; }

    public ICollection<FilterIntegrationType> IntegrationTypes { get; set; } = new HashSet<FilterIntegrationType>();

    public Filter Filter { get; set; }

    public Integration Integration { get; set; }
}
