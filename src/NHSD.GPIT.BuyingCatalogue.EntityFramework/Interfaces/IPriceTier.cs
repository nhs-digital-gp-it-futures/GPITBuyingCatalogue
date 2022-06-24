namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface IPriceTier
    {
        public int Id { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal Price { get; set; }

        public int Quantity => (UpperRange ?? int.MaxValue) - (LowerRange == 0 ? 0 : LowerRange - 1);

        public bool AppliesTo(int quantity) => quantity >= LowerRange && quantity <= (UpperRange ?? int.MaxValue);
    }
}
