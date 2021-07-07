namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class Integration
    {
        public string Link { get; set; }

        public string Name { get; set; }

        public IntegrationSubType[] SubTypes { get; set; }
    }
}
