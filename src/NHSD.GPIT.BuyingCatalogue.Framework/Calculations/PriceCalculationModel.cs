namespace NHSD.GPIT.BuyingCatalogue.Framework.Calculations
{
    public sealed class PriceCalculationModel
    {
        public PriceCalculationModel()
        {
        }

        public PriceCalculationModel(int id, int quantity, decimal price, decimal cost)
        {
            Id = id;
            Quantity = quantity;
            Price = price;
            Cost = cost;
        }

        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }
    }
}
