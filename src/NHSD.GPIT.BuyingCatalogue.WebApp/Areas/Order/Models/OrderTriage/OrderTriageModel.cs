using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage
{
    public class OrderTriageModel : NavBaseModel
    {
        public TriageOption? SelectedTriageOption { get; set; }

        public IEnumerable<SelectListItem> TriageOptions => new[]
        {
            new SelectListItem("Under £40k", TriageOption.Under40k.ToString()),
            new SelectListItem("£40k to £250k", TriageOption.Between40kTo250k.ToString()),
            new SelectListItem("Over £250k", TriageOption.Over250k.ToString()),
            new SelectListItem("I'm not sure", TriageOption.NotSure.ToString()),
        };
    }
}
