using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class SelectFlatDeclarativeQuantityModel : OrderingBaseModel
    {
        public SelectFlatDeclarativeQuantityModel()
        {
        }

        public SelectFlatDeclarativeQuantityModel(string odsCode, CallOffId callOffId, string solutionName, int? quantity)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date";
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
