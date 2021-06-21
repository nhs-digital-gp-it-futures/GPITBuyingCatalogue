using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class ListOrganisationsModel : NavBaseModel
    {
        public ListOrganisationsModel(IList<Organisation> organisations)
        {
            Organisations = organisations;
            BackLink = "/";
        }

        public IList<Organisation> Organisations { get; private set; }
    }
}
