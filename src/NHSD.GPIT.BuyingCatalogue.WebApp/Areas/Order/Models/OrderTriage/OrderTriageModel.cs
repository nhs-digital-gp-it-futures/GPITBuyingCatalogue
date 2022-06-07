using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage
{
    public class OrderTriageModel : NavBaseModel
    {
        public OrderTriageModel()
        {
        }

        public OrderTriageModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        public OrderTriageValue? SelectedOrderTriageValue { get; set; }

        public IEnumerable<SelectListItem> TriageOptions => new[]
        {
            new SelectListItem("Under £40k", OrderTriageValue.Under40K.ToString()),
            new SelectListItem("£40k to £250k", OrderTriageValue.Between40KTo250K.ToString()),
            new SelectListItem("Over £250k", OrderTriageValue.Over250K.ToString()),
            new SelectListItem("I'm not sure", OrderTriageValue.NotSure.ToString()),
        };
    }
}
