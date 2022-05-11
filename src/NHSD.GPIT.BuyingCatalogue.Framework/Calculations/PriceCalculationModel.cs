namespace NHSD.GPIT.BuyingCatalogue.Framework.Calculations
{
    public sealed class PriceCalculationModel
    {
        public PriceCalculationModel()
        {
        }

        public PriceCalculationModel(int id, int quantity, decimal cost)
        {
            Id = id;
            Quantity = quantity;
            Cost = cost;
        }

        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal Cost { get; set; }
    }
}
