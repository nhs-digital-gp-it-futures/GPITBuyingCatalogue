using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage
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

        public IEnumerable<SelectOption<string>> TriageOptions => new[]
        {
            new SelectOption<string>("Under £40k", OrderTriageValue.Under40K.ToString()),
            new SelectOption<string>("£40k to £250k", OrderTriageValue.Between40KTo250K.ToString()),
            new SelectOption<string>("Over £250k", OrderTriageValue.Over250K.ToString()),
            new SelectOption<string>("I'm not sure", OrderTriageValue.NotSure.ToString()),
        };
    }
}
