using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class InteroperabilityCriteria
{
    public InteroperabilityCriteria()
    {
    }

    public InteroperabilityCriteria(
        string qualifier,
        InteropIntegrationType integrationType)
    {
        Qualifier = qualifier;
        IntegrationType = integrationType;
    }

    public int Id { get; set; }

    public int NonPriceElementsId { get; set; }

    public string Qualifier { get; set; }

    public InteropIntegrationType IntegrationType { get; set; }
}
