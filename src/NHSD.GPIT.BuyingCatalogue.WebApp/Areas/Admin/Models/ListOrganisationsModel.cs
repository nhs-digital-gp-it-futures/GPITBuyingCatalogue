using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class ListOrganisationsModel : NavBaseModel
    {
        public ListOrganisationsModel(List<Organisation> organisations)
        {
            Organisations = organisations;
            BackLink = "/";
        }

        public List<Organisation> Organisations { get; private set;}
    }
}
