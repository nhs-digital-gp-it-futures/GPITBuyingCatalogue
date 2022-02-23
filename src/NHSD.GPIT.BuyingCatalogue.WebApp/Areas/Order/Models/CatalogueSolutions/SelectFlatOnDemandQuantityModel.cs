using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class SelectFlatOnDemandQuantityModel : OrderingBaseModel
    {
        public SelectFlatOnDemandQuantityModel()
        {
        }

        public SelectFlatOnDemandQuantityModel(string odsCode, CallOffId callOffId, string solutionName, int? quantity, TimeUnit? estimationPeriod)
        {
            Title = $"Quantity of {solutionName} for {callOffId}";
            InternalOrgId = odsCode;
            CallOffId = callOffId;
            SolutionName = solutionName;
            Quantity = quantity.ToString();
            EstimationPeriod = estimationPeriod;
        }

        public CallOffId CallOffId { get; set; }

        public string SolutionName { get; set; }

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

        public TimeUnit? EstimationPeriod { get; set; }

        public List<TimeUnit> TimeUnits { get; } = Enum.GetValues<TimeUnit>().ToList();
    }
}
