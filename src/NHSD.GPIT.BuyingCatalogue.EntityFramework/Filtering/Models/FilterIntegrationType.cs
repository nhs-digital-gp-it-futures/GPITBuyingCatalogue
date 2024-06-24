using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

public class FilterIntegrationType
{
    public FilterIntegrationType()
    {
    }

    public FilterIntegrationType(
        int integrationTypeId)
    {
        IntegrationTypeId = integrationTypeId;
    }

    public int FilterId { get; set; }

    public SupportedIntegrations IntegrationId { get; set; }

    public int IntegrationTypeId { get; set; }

    public Filter Filter { get; set; }

    public FilterIntegration Integration { get; set; }

    public IntegrationType IntegrationType { get; set; }
}
