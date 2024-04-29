using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class ReadyToStartModel : NavBaseModel
    {
        public ReadyToStartModel()
        {
        }

        public ReadyToStartModel(Organisation organisation)
        {
            InternalOrgId = organisation.InternalIdentifier;
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        public string InternalOrgId { get; set; }
    }
}
