namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface IPriceTier
    {
        public int Id { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal Price { get; set; }
    }
}
