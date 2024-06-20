namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class IntegrationType
{
    public int Id { get; set; }

    public int IntegrationId { get; set; }

    public string Name { get; set; }

    public Integration Integration { get; set; }
}
