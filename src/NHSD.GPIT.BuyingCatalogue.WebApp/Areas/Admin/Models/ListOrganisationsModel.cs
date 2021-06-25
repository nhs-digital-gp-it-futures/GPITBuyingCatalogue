using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class ListOrganisationsModel : NavBaseModel
    {
        public ListOrganisationsModel(IList<OrganisationModel> organisations)
        {
            Organisations = organisations;
        }

        public IList<OrganisationModel> Organisations { get; private set; }

        public Dictionary<string, string> BreadCrumb()
        {
            return new Dictionary<string, string>
            {
                { "Home", "/admin" },
            };
        }
    }
}
