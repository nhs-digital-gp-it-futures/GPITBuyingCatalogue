using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class SelectFlatOnDemandQuantityModel : OrderingBaseModel
    {
        public SelectFlatOnDemandQuantityModel()
        {
        }

        public SelectFlatOnDemandQuantityModel(string odsCode, string callOffId, string solutionName, int? quantity, EntityFramework.Models.Ordering.TimeUnit timeUnit)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date";
            BackLinkText = "Go back";
            Title = $"Quantity of {solutionName} for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            SolutionName = solutionName;
            Quantity = quantity.ToString();
            TimeUnit = timeUnit != null ? timeUnit.Name : string.Empty;
        }

        public string CallOffId { get; set; }

        public string SolutionName { get; set; }

        [Required(ErrorMessage = "Enter a quantity")]
        public string Quantity { get; set; }

        public string TimeUnit { get; set; }

        public List<EntityFramework.Models.Ordering.TimeUnit> TimeUnits
        {
            get
            {
                return new List<EntityFramework.Models.Ordering.TimeUnit>
                {
                    EntityFramework.Models.Ordering.TimeUnit.PerMonth,
                    EntityFramework.Models.Ordering.TimeUnit.PerYear,
                };
            }
        }

        public (int? Quantity, string Error) GetQuantity()
        {
            if (!int.TryParse(Quantity, out int quantity))
                return (null, "Quantity must be a number");

            if (quantity < 1)
                return (null, "Quantity must be greater than zero");

            return (quantity, null);
        }
    }
}
