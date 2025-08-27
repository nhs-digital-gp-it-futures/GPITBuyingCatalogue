namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public sealed class PriceCalculationModel
    {
        public PriceCalculationModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceCalculationModel"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor calls <seealso cref="PriceCalculationModel(int, int, decimal, decimal)"/> with a cost that is (quantity * price).
        /// </remarks>
        /// <param name="id">The ID of the pricing tier.</param>
        /// <param name="quantity">The quantity for this tier.</param>
        /// <param name="price">The price of this tier.</param>
        public PriceCalculationModel(
            int id,
            int quantity,
            decimal price)
            : this(id, quantity, price, price * quantity)
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
