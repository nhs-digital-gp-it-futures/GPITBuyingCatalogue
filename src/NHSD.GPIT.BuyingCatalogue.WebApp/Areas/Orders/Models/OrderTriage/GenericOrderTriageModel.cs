using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage
{
    public class GenericOrderTriageModel : NavBaseModel
    {
        public GenericOrderTriageModel()
        {
        }

        public GenericOrderTriageModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        public string InternalOrgId { get; set; }

        public string OrdersDashboardLink { get; set; }
    }
}
