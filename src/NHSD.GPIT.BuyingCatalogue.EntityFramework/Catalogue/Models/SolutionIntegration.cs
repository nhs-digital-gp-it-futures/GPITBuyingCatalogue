using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class SolutionIntegration
{
    public int Id { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public int IntegrationTypeId { get; set; }

    public string Description { get; set; }

    public bool IsConsumer { get; set; }

    public string IntegratesWith { get; set; }

    public Solution Solution { get; set; }

    public IntegrationType IntegrationType { get; set; }
}
