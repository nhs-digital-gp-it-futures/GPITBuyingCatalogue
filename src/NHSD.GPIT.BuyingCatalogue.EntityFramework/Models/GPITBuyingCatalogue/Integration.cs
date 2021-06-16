namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public class Integration
    {
        public string Link { get; set; }

        public string Name { get; set; }

        public IntegrationSubType[] SubTypes { get; set; }
    }
}
