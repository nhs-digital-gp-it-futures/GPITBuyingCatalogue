using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class IntegrationsCriteria
{
    public int NonPriceElementsId { get; set; }

    public int IntegrationTypeId { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public IntegrationType IntegrationType { get; set; }
}
