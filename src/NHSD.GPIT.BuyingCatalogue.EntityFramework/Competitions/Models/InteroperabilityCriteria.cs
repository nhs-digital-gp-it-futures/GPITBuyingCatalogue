using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class InteroperabilityCriteria
{
    public InteroperabilityCriteria()
    {
    }

    public InteroperabilityCriteria(
        string qualifier,
        SupportedIntegrations integrationType)
    {
        Qualifier = qualifier;
        IntegrationType = integrationType;
    }

    public int Id { get; set; }

    public int NonPriceElementsId { get; set; }

    public string Qualifier { get; set; }

    public SupportedIntegrations IntegrationType { get; set; }
}
