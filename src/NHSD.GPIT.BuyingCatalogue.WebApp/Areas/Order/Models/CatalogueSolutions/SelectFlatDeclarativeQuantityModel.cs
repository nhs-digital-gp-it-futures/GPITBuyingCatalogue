using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class SelectFlatDeclarativeQuantityModel : OrderingBaseModel
    {
        public SelectFlatDeclarativeQuantityModel()
        {
        }

        public SelectFlatDeclarativeQuantityModel(CallOffId callOffId, string solutionName, int? quantity)
        {
            Title = $"Quantity of {solutionName} for {callOffId}";
            Quantity = quantity.ToString();
        }

        public string Quantity { get; set; }

        public int? QuantityAsInt
        {
            get
            {
                if (!int.TryParse(Quantity, out var quantity))
                    return null;

                return quantity;
            }
        }
    }
}
