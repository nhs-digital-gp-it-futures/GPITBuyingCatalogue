namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CataloguePriceTier
    {
        public int Id { get; set; }

        public int CataloguePriceId { get; set; }

        public int BandStart { get; set; }

        public int? BandEnd { get; set; }

        public decimal Price { get; set; }
    }
}
