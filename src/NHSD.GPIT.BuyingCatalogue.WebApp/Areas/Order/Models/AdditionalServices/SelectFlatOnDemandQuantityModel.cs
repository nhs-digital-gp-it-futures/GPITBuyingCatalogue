using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class SelectFlatOnDemandQuantityModel : OrderingBaseModel
    {
        public SelectFlatOnDemandQuantityModel()
        {
        }

        public SelectFlatOnDemandQuantityModel(string odsCode, CallOffId callOffId, string solutionName, int? quantity, TimeUnit? timeUnit)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date";
            BackLinkText = "Go back";
            Title = $"Quantity of {solutionName} for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            SolutionName = solutionName;
            Quantity = quantity.ToString();
            TimeUnit = timeUnit;
        }

        public CallOffId CallOffId { get; set; }

        public string SolutionName { get; set; }

        [Required(ErrorMessage = "Enter a quantity")]
        public string Quantity { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public List<TimeUnit> TimeUnits { get; } = Enum.GetValues<TimeUnit>().ToList();

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
