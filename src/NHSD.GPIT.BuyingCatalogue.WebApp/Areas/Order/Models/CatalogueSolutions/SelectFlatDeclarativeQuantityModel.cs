using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Enter a quantity")]
        public string Quantity { get; set; }

        public (int? Quantity, string Error) GetQuantity()
        {
            if (!int.TryParse(Quantity, out var quantity))
                return (null, "Quantity must be a number");

            if (quantity < 1)
                return (null, "Quantity must be greater than zero");

            return (quantity, null);
        }
    }
}
